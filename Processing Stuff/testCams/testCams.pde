import processing.video.*;

Capture cam1, cam2, cam3, cam4;

void setup() 
{
  fullScreen();
  String[] cameras = Capture.list();

  if (cameras.length == 0) 
  {
    println("There are no cameras available for capture.");
    exit();
  } else 
  {
    println("Available cameras:");
    printArray(cameras);

    cam1 = new Capture(this, 864, 480, "Cam1", 30);
    cam1.start();

    cam2 = new Capture(this, 864, 480, "Cam2", 30);
    cam2.start();

    cam3 = new Capture(this, 864, 480, "Cam3", 30);
    cam3.start();

    cam4 = new Capture(this, 864, 480, "Cam4", 30);
    cam4.start();
  }
}
void draw() 
{
  if (cam1.available() && cam2.available() && cam3.available() && cam4.available()) 
  {
    cam1.read();
    cam2.read();
    cam3.read();
    cam4.read();

    PImage c1 = cam1;
    PImage c2 = cam2;
    PImage c3 = cam3;
    PImage c4 = cam4;
    
    PGraphics output = createGraphics(width, height, JAVA2D);
    
output.beginDraw();
    output.image(c1, 0, 0, width*0.5, height*0.5);
    output.image(c2, width*0.5, 0, width*0.5, height*0.5);
    output.image(c3, 0, height*0.5, width*0.5, height*0.5);
    output.image(c4, width*0.5, height*0.5, width*0.5, height*0.5);
    output.endDraw();

    strokeWeight(2);

    drawLines("Not really cam1", 0, 0.5, 0.25, 0, 0.5, 0.25);
    drawLines("Not really cam2", 0.5, 1, 0.25, 0, 0.5, 0.75);
    drawLines("Not really cam3", 0, 0.5, 0.75, 0.5, 1, 0.25);
    drawLines("Not really cam4", 0.5, 1, 0.75, 0.5, 1, 0.75);

    strokeWeight(5);
    stroke(255, 0, 0);
    line(width*0.5, 0, width*0.5, height);
    line(0, height*0.5, width, height*0.5);
  }
}

void drawLines(String name, float horiX1, float horiX2, float horiY, float vertiY1, float vertiY2, float vertiX) 
{
  fill(255);
  text(name, width*horiX1+10, height*vertiY1+30);

  stroke(0, 150, 255);
  line(width*horiX1, height*horiY, width*horiX2, height*horiY);

  stroke(150, 255, 0);
  line(width*vertiX, height*vertiY1, width*vertiX, height*vertiY2);
}