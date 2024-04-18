# import the necessary packages
import cv2 #image manpiulation
import os #file access
import platform
import sqlite3 #execute sql
import re
import subprocess #execute linux commands
import imageio.v2 as imageio #trying this to open images, cv2 stopped working in linux :-(
import pytesseract

import time

from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler #event handler to listen for when an image is added to the folder
from sqlite3 import Error

pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR/tesseract.exe'

database = ""
updateFile = ""
fileSeperator = ""

### CREATE READ WHEN PHOTO IS ADDED TO FOLDER
class PhotoHandler(FileSystemEventHandler):
    def on_created(self, event):
        if event.is_directory:
            return
        elif event.event_type == 'created' and event.src_path.lower().endswith('png'):
            try:
                global fileSeperator
                meter = event.src_path.split(fileSeperator)[-1]# / for linux, \\ for windows
                properties = meter.split('_')
                meterId = properties[0]
                controllerId = properties[1]
            
                datetime = properties[2].split(' ')
                date = datetime[0]
                time = datetime[1].split('.')[0]
                time = f"{time[:2]}:{time[2:4]}:{time[4:]}"
            
                datetime = f"{date} {time}"
                #reading = read_meter_ssocr(event.src_path)
                reading = read_meter_old(event.src_path)
                newReading = (meterId,controllerId,datetime, reading)
                insert_reading(newReading)
                print("Inserted new Reading")
            except Error as e:
                print(f'failed to read')
                print(e)

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
    conn.close()


    f = open(updateFile, "w")
    f.write("Updated")
    f.close()

### READ METER ALGORITHM
def read_meter_ssocr(meterPhoto):
    image = imageio.imread(meterPhoto)
    crop_image = image[935:1066, 1122:1590]#image[415:485, 385:665] #old values
    rgb = cv2.cvtColor(crop_image, cv2.COLOR_BGR2RGB)
    cv2.imwrite(meterPhoto, rgb)
    ##OCR the input image using ssocr
    output = subprocess.Popen(['ssocr', '-T', meterPhoto.split('/')[-1]], stdout=subprocess.PIPE)
    value = output.communicate()[0].decode("utf-8")
    return float(value)

### OLD READ METER ALGORITHM (USING NON SEVEN-SEGMENT IMAGES)
def read_meter_old(meterPhoto):
    image = cv2.imread(meterPhoto)
    crop_image = image[415:485, 385:665]
    rgb = cv2.cvtColor(crop_image, cv2.COLOR_BGR2RGB)
    # OCR the input image using Tesseract
    return float(pytesseract.image_to_string(rgb, config=""))

### MAIN METHOD
def main():
    global fileSeperator

    match platform.system():
        case 'Windows': fileSeperator = '\\'
        case 'Linux': fileSeperator = '/'

    ### GET DATABASE FILE
    current_directory = os.getcwd()
    relative_path = os.path.join('..')
    absolute_path = os.path.abspath(os.path.join(current_directory, relative_path))

    dbFolder = "Data"
    
    global database
    database = os.path.join(absolute_path, dbFolder, 'EnergyHub.db')
    
    global updateFile
    updateFile = os.path.join(absolute_path, dbFolder, 'update.txt')

    meterImagePath = current_directory #os.path.join(absolute_path, 'ArtificialMeters')

    photo_handler = PhotoHandler()

    print(f'Database: {database}')
    print(f'Update File: {updateFile}')
    print(f'Image Path: {meterImagePath}')

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