import oscP5.*;
import netP5.*;
import processing.video.*;
import gab.opencv.*;
import java.awt.Rectangle;
import java.awt.geom.AffineTransform;
import java.awt.image.AffineTransformOp;
import java.awt.image.BufferedImage;

//import processing.video.*;

//Osc
Capture cam1, cam2, cam3, cam4;
OscP5 oscP5;
NetAddress myBroadcastLocation;

//OpenCV
OpenCV opencv;
PImage src, dst;
ArrayList<Contour> contours;
ArrayList<Contour> newBlobContours;// List of detected contours parsed as blobs (every frame)
ArrayList<Blob> blobList;
float contrast = .9;
int blobSizeThreshold = 10;
int threshold = 175;
int blurSize = 4;
int blobCount = 0;

void setup() {
  fullScreen();
  //size(1280, 800);
  blobList = new ArrayList<Blob>();
  // frameRate(20);
  oscP5 = new OscP5(this, 12000);
  myBroadcastLocation = new NetAddress("127.0.0.1", 8000);

  String[] cameras = Capture.list();

  if (cameras.length == 0) {
    println("There are no cameras available for capture.");
    exit();
  } else {
    println("Available cameras:");
    printArray(cameras);

    //cam1 = new Capture(this, cameras[199]);
    cam1 = new Capture(this, 864, 480, "Cam1", 30);
    cam1.start();

    //cam2 = new Capture(this, cameras[111]);
    cam2 = new Capture(this, 864, 480, "Cam2", 30);
    cam2.start();

    //cam3 = new Capture(this, cameras[287]);
    cam3 = new Capture(this, 864, 480, "Cam3", 30);
    cam3.start();

    //cam4 = new Capture(this, cameras[23]);
    cam4 = new Capture(this, 864, 480, "Cam4", 30);
    cam4.start();
  }  
  //delay(250);
  opencv = new OpenCV(this, 1280, 800);
  noStroke();
  smooth();
}

void draw() {

  if (cam1.available() && cam2.available() && cam3.available() && cam4.available()) {
    cam1.read();
    cam2.read();
    cam3.read();
    cam4.read();

    PImage c1 = cam1;
    PImage c2 = cam2;
    PImage c3 = cam3;
    PImage c4 = cam4;
    /*c1 = flipImage(c1);
     c2 = flipImage(c2);
     c3 = flipImage(c3);
     c4 = flipImage(c4);*/


    //image(cam, 0, 0);
    //src = cam1

    PGraphics output = createGraphics(1280, 800, JAVA2D);
    output.beginDraw();

    //pushMatrix();
    //translate(width/2, height/2);
    //rotate(PI/2);

    output.image(c1, 0, 0, 640, height*0.5);
    output.image(c2, 640, 0, 640, height*0.5);
    output.image(c3, 0, 400, 640, height*0.5);
    output.image(c4, 640, 400, 640, height*0.5);
    output.endDraw();
    image(output, 0, 0);

    opencv.loadImage(output.get());

    opencv.gray();
    opencv.contrast(contrast);
    opencv.threshold(threshold);

    opencv.dilate();
    opencv.erode();

    opencv.blur(blurSize);

    dst = opencv.getOutput();

    detectBlobs();
    //contours = opencv.findContours();
    //println("found " + contours.size() + " contours");
    src = opencv.getSnapshot();
    //scale(.5);
    displayBlobs();
    image(src, 0, 0);
    //image(dst, src.width, 0);
    displayBlobs();
    //    filter(THRESHOLD, .5);

    //print(blobList.size());
    for (int i = 0; i < blobList.size(); i++) {
      if (!blobList.get(i).dead()) {
        //print("im alive");
        Rectangle r = blobList.get(i).getBoundingBox();
        sendMessage(r.x, r.y, r.width*r.height, blobList.get(i).id);
        //print("squared: " ,r.width*r.height);
      }
    }



    noFill();
    strokeWeight(3);

    for (Contour contour : contours) {
      stroke(0, 255, 0);
      //contour.draw();

      stroke(255, 0, 0);
      beginShape();
      for (PVector point : contour.getPolygonApproximation().getPoints()) {
        vertex(point.x, point.y);
      }
      endShape();
    }
    text("Framerate: " + int(frameRate), 10, 450);
  }

  line(640, 0, 640, 800);
  line(0, 400, 1280, 400);
}

/*
private PImage flipImage(PImage i) {
 int iWidth = 640;
 int iHeight = 400;
 
 //BufferedImage bimg = new BufferedImage(iWidth, iHeight, BufferedImage.TYPE_INT_RGB);
 BufferedImage bimg = (BufferedImage) i.getNative();
 //Flip
 AffineTransform tx = AffineTransform.getScaleInstance(-1, 1);
 tx.translate(-iWidth, 0);
 AffineTransformOp op = new AffineTransformOp(tx, AffineTransformOp.TYPE_NEAREST_NEIGHBOR);
 bimg = op.filter(bimg, null);
 
 //Convert to PImage
 PImage img=new PImage(bimg.getWidth(), bimg.getHeight(), PConstants.ARGB);
 bimg.getRGB(0, 0, iWidth, iHeight, img.pixels, 0, iWidth);
 img.updatePixels();
 return img;
 }
 */

void sendMessage(int xx, int zz, int area, int id) {
  print(area + " ");

  /*  if (xx<=640) {
   xx = (int)map(xx, 0, 640, 640, 0);
   }
   if (xx>640) {
   xx = (int)map(xx, 640, 1280, 1280, 640);
   }*/

  if (xx<=640) {
    xx = (int)map(xx, 0, 640, 640, 1280);
  } else if (xx>640) {
    xx = (int)map(xx, 640, 1280, 0, 640);
  }

  float mappedX = map(xx, 1280, 0, 10, 86);//10,85
  float mappedZ = map(zz, 0, 800, 55, 0);//40,4
  float mappedArea = map(area, 1500, 27000, 3, 7);//check med blobs
  //println(xx, zz, area, mappedX, mappedZ, mappedArea);
  //drawText(mappedX, mappedZ, mappedArea, xx, zz);

  OscMessage myOscMessage = new OscMessage("/positionData");
  myOscMessage.add(mappedX);
  myOscMessage.add(mappedZ);
  myOscMessage.add(mappedArea);
  myOscMessage.add(id);
  //print(id);
  oscP5.send(myOscMessage, myBroadcastLocation);
  println(xx, mappedX);
}

/*void sendMessage(int x, int z, int area, int num) {
 float mappedX = map(x, 0, 640, 21, 107);
 float mappedZ = map(z, 0, 400, 83, 44);
 float mappedArea = map(area, 500, 16000,1,10);
 
 OscMessage myOscMessage = new OscMessage("/positionData");
 myOscMessage.add(mappedX);
 myOscMessage.add(mappedZ);
 myOscMessage.add(mappedArea);
 //myOscMessage.add(num);
 oscP5.send(myOscMessage, myBroadcastLocation);
 }*/

void displayContoursBoundingBoxes() {

  for (int i=0; i<contours.size(); i++) {

    Contour contour = contours.get(i);
    Rectangle r = contour.getBoundingBox();

    if (//(contour.area() > 0.9 * src.width * src.height) ||
      (r.width < blobSizeThreshold || r.height < blobSizeThreshold))
      continue;

    stroke(255, 0, 0);
    fill(255, 0, 0, 150);
    strokeWeight(2);

    rect(r.x-r.width, r.y, r.width, r.height);
  }
}

////////////////////
// Blob Detection
////////////////////

void detectBlobs() {

  // Contours detected in this frame
  // Passing 'true' sorts them by descending area.
  contours = opencv.findContours(true, true);

  newBlobContours = getBlobsFromContours(contours);

  //println(contours.length);

  // Check if the detected blobs already exist are new or some has disappeared. 

  // SCENARIO 1 
  // blobList is empty
  if (blobList.isEmpty()) {
    // Just make a Blob object for every face Rectangle
    for (int i = 0; i < newBlobContours.size(); i++) {
      println("+++ New blob detected with ID: " + blobCount);
      blobList.add(new Blob(this, blobCount, newBlobContours.get(i)));
      blobCount++;
    }

    // SCENARIO 2 
    // We have fewer Blob objects than face Rectangles found from OpenCV in this frame
  } else if (blobList.size() <= newBlobContours.size()) {
    boolean[] used = new boolean[newBlobContours.size()];
    // Match existing Blob objects with a Rectangle
    for (Blob b : blobList) {
      // Find the new blob newBlobContours.get(index) that is closest to blob b
      // set used[index] to true so that it can't be used twice
      float record = 50000;
      int index = -1;
      for (int i = 0; i < newBlobContours.size(); i++) {
        float d = dist(newBlobContours.get(i).getBoundingBox().x, newBlobContours.get(i).getBoundingBox().y, b.getBoundingBox().x, b.getBoundingBox().y);
        //float d = dist(blobs[i].x, blobs[i].y, b.r.x, b.r.y);
        if (d < record && !used[i]) {
          record = d;
          index = i;
        }
      }
      // Update Blob object location
      used[index] = true;
      b.update(newBlobContours.get(index));
    }
    // Add any unused blobs
    for (int i = 0; i < newBlobContours.size(); i++) {
      if (!used[i]) {
        println("+++ New blob detected with ID: " + blobCount);
        blobList.add(new Blob(this, blobCount, newBlobContours.get(i)));
        //blobList.add(new Blob(blobCount, blobs[i].x, blobs[i].y, blobs[i].width, blobs[i].height));
        blobCount++;
      }
    }

    // SCENARIO 3 
    // We have more Blob objects than blob Rectangles found from OpenCV in this frame
  } else {
    // All Blob objects start out as available
    for (Blob b : blobList) {
      b.available = true;
    } 
    // Match Rectangle with a Blob object
    for (int i = 0; i < newBlobContours.size(); i++) {
      // Find blob object closest to the newBlobContours.get(i) Contour
      // set available to false
      float record = 50000;
      int index = -1;
      for (int j = 0; j < blobList.size(); j++) {
        Blob b = blobList.get(j);
        float d = dist(newBlobContours.get(i).getBoundingBox().x, newBlobContours.get(i).getBoundingBox().y, b.getBoundingBox().x, b.getBoundingBox().y);
        //float d = dist(blobs[i].x, blobs[i].y, b.r.x, b.r.y);
        if (d < record && b.available) {
          record = d;
          index = j;
        }
      }
      // Update Blob object location
      Blob b = blobList.get(index);
      b.available = false;
      b.update(newBlobContours.get(i));
    } 
    // Start to kill any left over Blob objects
    for (Blob b : blobList) {
      if (b.available) {
        b.countDown();
        if (b.dead()) {
          b.delete = true;
        }
      }
    }
  }
}

ArrayList<Contour> getBlobsFromContours(ArrayList<Contour> newContours) {

  ArrayList<Contour> newBlobs = new ArrayList<Contour>();

  // Which of these contours are blobs?
  for (int i=0; i<newContours.size(); i++) {

    Contour contour = newContours.get(i);
    Rectangle r = contour.getBoundingBox();

    if (//(contour.area() > 0.9 * src.width * src.height) ||
      (r.width < blobSizeThreshold || r.height < blobSizeThreshold))
      continue;

    newBlobs.add(contour);
  }

  return newBlobs;
}

void displayBlobs() {

  for (Blob b : blobList) {
    strokeWeight(1);
    b.display();
  }
}

/*int brightestX = 0; // X-coordinate of the brightest video pixel
 int brightestY = 0; // Y-coordinate of the brightest video pixel
 float brightestValue = 100; // Brightness of the brightest video pixel
 // Search for the brightest pixel: For each row of pixels in the video image and
 // for each pixel in the yth row, compute each pixel's index in the video
 cam.loadPixels();
 int index = 0;
 boolean changes = false;
 for (int y = 0; y < cam.height; y++) 
 {
 for (int x = 0; x < cam.width; x++) 
 {
 // Get the color stored in the pixel
 int pixelValue = cam.pixels[index];
 // Determine the brightness of the pixel
 float pixelBrightness = brightness(pixelValue);
 // If that value is brighter than any previous, then store the
 // brightness of that pixel, as well as its (x,y) location
 if (pixelBrightness > brightestValue) 
 {
 brightestValue = pixelBrightness;
 brightestY = y;
 brightestX = x;
 sendMessage(x, y);
 changes = true;
 //println("Brightness value = " + pixelBrightness);
 //println("X = " + x);
 //println("Y = " + y);
 }
 index++;
 }
 
 // Draw a large, yellow circle at the brightest pixel
 fill(255, 204, 0, 128);
 ellipse(brightestX, brightestY, 50, 50);
 
 
 fill(255);
 text("Framerate: " + int(frameRate), 10, 450);
 }
 if (!changes) {
 sendMessage(0, 0);
 }*/


// The following does the same, and is faster when just drawing the image
// without any additional resizing, transformations, or tint.
//set(0, 0, cam);