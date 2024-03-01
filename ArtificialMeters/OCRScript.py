# import the necessary packages
import cv2 #image manpiulation
import os #file access
import sqlite3 #execute sql
import re
import subprocess #execute linux commands
import imageio.v2 as imageio #trying this to open images, cv2 stopped working in linux :-(

import time

from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler #event handler to listen for when an image is added to the folder
from sqlite3 import Error

meterImages = ""
database = ""

### CREATE READ WHEN PHOTO IS ADDED TO FOLDER
class PhotoHandler(FileSystemEventHandler):
    def on_created(self, event):
        if event.is_directory:
            return
        elif event.event_type == 'created' and event.src_path.lower().endswith('jpg'):
            print(f'read {event.src_path}')
            meter = event.src_path.split('/')[-1]# / for linux, \\ for windows
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
    image = imageio.imread(meterPhoto)
    #cv2.imshow("image",image)
    #crop_image = image[415:485, 385:665] #old values
    crop_image = image[935:1066, 1122:1590]
    rgb = cv2.cvtColor(crop_image, cv2.COLOR_BGR2RGB)
    cv2.imwrite(meterPhoto, rgb)
    ##OCR the input image using ssocr
    output = subprocess.Popen(['ssocr', '-T', meterPhoto.split('/')[-1]], stdout=subprocess.PIPE)
    value = output.communicate()[0].decode("utf-8")
    return float(value)
    
    # OCR the input image using Tesseract
    #return float(pytesseract.image_to_string(rgb, config=""))


def main():
    ### GET DATABASE FILE
    current_directory = os.getcwd()
    relative_path = os.path.join('..')
    absolute_path = os.path.abspath(os.path.join(current_directory, relative_path))
    
    database = os.path.join(absolute_path, 'EnergyInsightHub', 'Data', 'EnergyHub.db')
    meterImagePath = current_directory #os.path.join(absolute_path, 'ArtificialMeters')

    #meterImages = os.listdir(meterImagePath)

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