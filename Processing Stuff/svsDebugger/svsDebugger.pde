import oscP5.*;
import netP5.*;

OscP5 oscP5;
NetAddress myBroadcastLocation;
int x, z, sZ, sX;
int str = 7000;
int counter = 0;
void setup() {
  size(300, 300);
  background(0);
  textSize(32);
  
  oscP5 = new OscP5(this, 12000);
  myBroadcastLocation = new NetAddress("127.0.0.1", 8000);
  x = (int)random(0, 640);
  z = (int)random(0, 420);
  sZ = 1;
  sX = -1;
}

void draw() {
  
  
  if (keyPressed) {
    if (key == 's') {
      x+= sX;
      z+= sZ;
      //print((int)random(100)%17, "");
      if ((random(100)%17) == 1) {
        sZ = -sZ;
        print(sZ);
      }
      if ((random(100)%17) == 1)
        sX = -sX;
      if (x > 640 )
        x = (int)random(640);
      if (z > 420)
        z = (int)random(420);
      sendMessage(x, z, str, 0);
    }
    //Corner checks -----------------------------------------------
    if (key == '1')
      sendMessage(0, 0, str, 0);
    if (key == '2')
      sendMessage(640, 0, str, 0);
    if (key == '3')
      sendMessage(0, 420, str, 0);
    if (key == '4')
      sendMessage(640, 420, str, 0);

    //IJKL Controls -----------------------------------------------
    if (key == 'j') {
      x--;
      sendMessage(x, z, str, 0);
    }
    if (key == 'l') {
      x++;
      sendMessage(x, z, str, 0);
    }
    if (key == 'i') {
      z--;
      sendMessage(x, z, str, 0);
    }
    if (key == 'k') {
      z++;
      sendMessage(x, z, str, 0);
    }
    
    //Test Multiple
    if (key == 'd'){
      if(counter%2 == 1)
        sendMessage(170,220,str,0);
      else
        sendMessage(470,220,str,0);
      counter++;
    }
  }
  if (x > 640 || x < 0 )
    x = (int)random(640);
  if (z > 420 || x < 0)
    z = (int)random(420);
    
  
}

void sendMessage(int xx, int zz, int area, int num) {
  /*float mappedX = map(xx, 640, 0, 0, 90);
  float mappedZ = map(zz, 0, 480,60, 10);
    */
  float mappedX = map(xx, 640, 0, 10, 85);
  float mappedZ = map(zz, 0, 480,50, 4);
  float mappedArea = map(area, 500, 16000, 1, 10);//check med blobs
  drawText(mappedX, mappedZ, mappedArea, xx, zz);

  OscMessage myOscMessage = new OscMessage("/positionData");
  myOscMessage.add(mappedX);
  myOscMessage.add(mappedZ);
  myOscMessage.add(mappedArea);
  //myOscMessage.add(num);
  oscP5.send(myOscMessage, myBroadcastLocation);
}

void drawText(float w1, float w2, float w3, float xx, float zz) {
  background(0);
  fill(0, 102, 153);
  text("mX:"+w1, 10, 30);
  text("X:"+xx, 10, 60);
  text("mY:"+w2, 10, 90);
  text("Y:"+zz, 10, 120);
  text("A:"+w3, 10, 290);
}