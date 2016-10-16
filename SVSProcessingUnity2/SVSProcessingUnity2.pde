import oscP5.*;
import netP5.*;
import processing.video.*;

Capture cam;
OscP5 oscP5;
NetAddress myBroadcastLocation;

void setup() {
  size(640, 480);
  frameRate(20);
  oscP5 = new OscP5(this, 12000);
  myBroadcastLocation = new NetAddress("127.0.0.1", 8000);

  String[] cameras = Capture.list();

  if (cameras.length == 0) {
    println("There are no cameras available for capture.");
    exit();
  } else {
    println("Available cameras:");
    printArray(cameras);

    cam = new Capture(this, cameras[1]);
    cam.start();
    
  }  
  //delay(250);
  noStroke();
  smooth();
}

void draw() {
  if (cam.available() == true) {
    cam.read();
  }
  image(cam, 0, 0);
  int brightestX = 0; // X-coordinate of the brightest video pixel
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
        /* println("Brightness value = " + pixelBrightness);
         println("X = " + x);
         println("Y = " + y);*/
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
    }


  // The following does the same, and is faster when just drawing the image
  // without any additional resizing, transformations, or tint.
  //set(0, 0, cam);
}

void sendMessage(int x, int z) {
  float mappedX = map(x, 0, 640, 21, 107);
  float mappedZ = map(z, 0, 480, 83, 44);
  OscMessage myOscMessage = new OscMessage("/positionData");
  myOscMessage.add(mappedX);
  myOscMessage.add(mappedZ);
  oscP5.send(myOscMessage, myBroadcastLocation);
}