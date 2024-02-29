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

### GET DATABASE FILE
current_directory = os.getcwd()
relative_path = os.path.join('..', '..')
absolute_path = os.path.abspath(os.path.join(current_directory, relative_path))
database = absolute_path + r'\EnergyInsightHub\Data\EnergyHub.db'
meterImagePath = absolute_path + r'\ArtificialMeters'
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




###OLD SOLUTION
# import the necessary packages
# from re import T
# from imutils.perspective import four_point_transform
# from imutils import contours
# import imutils
# import cv2
# # define the dictionary of digit segments so we can identify
# # each digit on the thermostat
# DIGITS_LOOKUP = {
# 	(1, 1, 1, 0, 1, 1, 1): 0,
# 	(0, 0, 1, 0, 0, 1, 0): 1,
# 	(1, 0, 1, 1, 1, 1, 0): 2,
# 	(1, 0, 1, 1, 0, 1, 1): 3,
# 	(0, 1, 1, 1, 0, 1, 0): 4,
# 	(1, 1, 0, 1, 0, 1, 1): 5,
# 	(1, 1, 0, 1, 1, 1, 1): 6,
# 	(1, 0, 1, 0, 0, 1, 0): 7,
# 	(1, 1, 1, 1, 1, 1, 1): 8,
# 	(1, 1, 1, 1, 0, 1, 1): 9
# }

# # load the example image
# image = cv2.imread("example.jpg")
# #image = cv2.imread("example.jpg")
# # pre-process the image by resizing it, converting it to
# # graycale, blurring it, and computing an edge map
# image = imutils.resize(image, height=500)
# gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
# blurred = cv2.GaussianBlur(gray, (5, 5), 0)
# edged = cv2.Canny(blurred, 50, 200, 255)

# # find contours in the edge map, then sort them by their
# # size in descending order
# cnts = cv2.findContours(edged.copy(), cv2.RETR_EXTERNAL,
# 	cv2.CHAIN_APPROX_SIMPLE)
# cnts = imutils.grab_contours(cnts)
# cnts = sorted(cnts, key=cv2.contourArea, reverse=True)

# displayCnt = None
# #loop over the contours
# for c in cnts:
# 	# approximate the contour
# 	peri = cv2.arcLength(c, True)
# 	approx = cv2.approxPolyDP(c, 0.02 * peri, True)
# 	# if the contour has four vertices, then we have found
# 	# the thermostat display
# 	if len(approx) == 4:
# 		displayCnt = approx
# 		break

# # extract the thermostat display, apply a perspective transform
# # to it
# warped = four_point_transform(gray, displayCnt.reshape(4, 2))
# output = four_point_transform(image, displayCnt.reshape(4, 2))


# # threshold the warped image, then apply a series of morphological
# # operations to cleanup the thresholded image
# thresh = cv2.threshold(warped, 0, 255,
# 	cv2.THRESH_BINARY_INV | cv2.THRESH_OTSU)[1]
# kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (1, 5))
# thresh = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel)

# #imagem = cv2.bitwise_not(thresh)
# cv2.imshow("Output", thresh)
# cv2.waitKey(0)



# # find contours in the thresholded image, then initialize the
# # digit contours lists
# cnts = cv2.findContours(thresh.copy(), cv2.RETR_EXTERNAL,
# 	cv2.CHAIN_APPROX_SIMPLE)
# cnts = imutils.grab_contours(cnts)

# digitCnts = []
# # loop over the digit area candidates
# for c in cnts:
# 	# compute the bounding box of the contour
# 	(x, y, w, h) = cv2.boundingRect(c)
# 	# if the contour is sufficiently large, it must be a digit
# 	if w >= 15 and (h >= 30 and h <= 40):
# 		digitCnts.append(c)

# # sort the contours from left-to-right, then initialize the
# # actual digits themselves
# digitCnts = contours.sort_contours(digitCnts,
# 	method="left-to-right")[0]
# digits = []


# # loop over each of the digits
# for c in digitCnts:
# 	# extract the digit ROI
# 	(x, y, w, h) = cv2.boundingRect(c)
# 	roi = thresh[y:y + h, x:x + w]
# 	# compute the width and height of each of the 7 segments
# 	# we are going to examine
# 	(roiH, roiW) = roi.shape
# 	(dW, dH) = (int(roiW * 0.25), int(roiH * 0.15))
# 	dHC = int(roiH * 0.05)
# 	# define the set of 7 segments
# 	segments = [
# 		((0, 0), (w, dH)),	# top
# 		((0, 0), (dW, h // 2)),	# top-left
# 		((w - dW, 0), (w, h // 2)),	# top-right
# 		((0, (h // 2) - dHC) , (w, (h // 2) + dHC)), # center
# 		((0, h // 2), (dW, h)),	# bottom-left
# 		((w - dW, h // 2), (w, h)),	# bottom-right
# 		((0, h - dH), (w, h))	# bottom
# 	]
# 	on = [0] * len(segments)
	
# 	# loop over the segments
# 	for (i, ((xA, yA), (xB, yB))) in enumerate(segments):
# 		# extract the segment ROI, count the total number of
# 		# thresholded pixels in the segment, and then compute
# 		# the area of the segment
# 		segROI = roi[yA:yB, xA:xB]
# 		total = cv2.countNonZero(segROI)
# 		area = (xB - xA) * (yB - yA)
# 		# if the total number of non-zero pixels is greater than
# 		# 50% of the area, mark the segment as "on"
# 		if total / float(area) > 0.5:
# 			on[i]= 1
# 	# lookup the digit and draw it on the image
# 	digit = DIGITS_LOOKUP[tuple(on)]
# 	digits.append(digit)
# 	cv2.rectangle(output, (x, y), (x + w, y + h), (0, 255, 0), 1)
# 	cv2.putText(output, str(digit), (x - 10, y - 10),
# 		cv2.FONT_HERSHEY_SIMPLEX, 0.65, (0, 255, 0), 2)
	
# # display the digits
# print(u"{}{}.{} \u00b0C".format(*digits))
# cv2.imshow("Input", image)
# cv2.imshow("Output", output)
# cv2.waitKey(0)