# import the necessary packages
import pytesseract #OCR read text from images
import cv2 #image manpiulation
import os #file access
import sqlite3 #execute sql
import re

import time

from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler #event handler to listen for when an image is added to the folder
from sqlite3 import Error

pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR/tesseract.exe'

### CREATE READ WHEN PHOTO IS ADDED TO FOLDER
class PhotoHandler(FileSystemEventHandler):
    def on_created(self, event):
        if event.is_directory:
            return
        elif event.event_type == 'created' and event.src_path.lower().endswith('png'):
            print(f'read {event.src_path}')
            meter = event.src_path.split('\\')[-1]
            properties = meter.split('_')
            meterId = properties[0]
            controllerId = properties[1]
        
            datetime = properties[2].split(' ')
            date = datetime[0]
            time = datetime[1].split('.')[0]
            time = f"{time[:2]}:{time[2:4]}:{time[4:]}"
        
            datetime = f"{date} {time}"

            reading = read_meter(event.src_path)
        
            newReading = (meterId,controllerId,datetime, reading)
            insert_reading(newReading)

### CREATE CONNECTION WITH DATEABASE FILE
def create_connection(db_file):
    """ create a database connection to the SQLite database
        specified by db_file
    :param db_file: database file
    :return: Connection object or None
    """
    conn = None
    try:
        conn = sqlite3.connect(db_file)
    except Error as e:
        print(e)

    return conn

## INSERT READING INTO DATABASE
def insert_reading(reading):
    conn = create_connection(database)
    with conn:
        sql = ''' INSERT INTO Readings(EnergyMeterId,MicrocontrollerId,Time,Amount)
                  VALUES(?,?,?,?) '''
        cur = conn.cursor()
        cur.execute(sql, reading)
        conn.commit()

### READ METER ALGORITHM
def read_meter(meterPhoto):
    image = cv2.imread(meterPhoto)
    crop_image = image[415:485, 385:665]
    rgb = cv2.cvtColor(crop_image, cv2.COLOR_BGR2RGB)
    options = "" #"outputbase digits"
    # OCR the input image using Tesseract
    return float(pytesseract.image_to_string(rgb, config=options))


def main():
    ### GET DATABASE FILE
    current_directory = os.getcwd()
    relative_path = os.path.join('..')
    absolute_path = os.path.abspath(os.path.join(current_directory, relative_path))
    
    database = os.path.join(absolute_path, 'EnergyInsightHub', 'Data', 'EnergyHub.db')

    meterImagePath = os.path.join(absolute_path, 'ArtificialMeters')
    
    print(meterImagePath)
    meterImages = os.listdir(meterImagePath)

    photo_handler = PhotoHandler()

    print('service running...')
    observer = Observer()
    observer.schedule(photo_handler, path = meterImagePath)
    observer.start()

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        observer.stop()
        
    observer.join()

if __name__ == "__main__":
    main()