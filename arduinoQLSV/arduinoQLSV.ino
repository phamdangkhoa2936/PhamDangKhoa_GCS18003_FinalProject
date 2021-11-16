

#include <Adafruit_Fingerprint.h>


#if (defined(__AVR__) || defined(ESP8266)) && !defined(__AVR_ATmega2560__)
// For UNO and others without hardware serial, we must use software serial...
// pin #2 is IN from sensor (GREEN wire)
// pin #3 is OUT from arduino  (WHITE wire)
// Set up the serial port to use softwareserial..
SoftwareSerial mySerial(2, 3);

#else
// On Leonardo/M0/etc, others with hardware serial, use hardware serial!
// #0 is green wire, #1 is white
#define mySerial Serial1

#endif


Adafruit_Fingerprint finger = Adafruit_Fingerprint(&mySerial);

uint8_t id;
char bdoi = 0;
void dangkysv();
uint8_t getFingerprintEnroll();
void diemdanhsv();
uint8_t getFingerprintID();
void setup()
{
  Serial.begin(9600);
  while (!Serial);  // For Yun/Leo/Micro/Zero/...
  delay(50);
  Serial.println("\n\nAdafruit Fingerprint sensor enrollment");

  // set the data rate for the sensor serial port
  finger.begin(57600);

  if (finger.verifyPassword()) {
    Serial.println("Found fingerprint sensor!");
  } else {
    Serial.println("Did not find fingerprint sensor :(");
    while (1) { delay(1); }
  }

  Serial.println(F("Reading sensor parameters"));
  finger.getParameters();
  Serial.print(F("Status: 0x")); Serial.println(finger.status_reg, HEX);
  Serial.print(F("Sys ID: 0x")); Serial.println(finger.system_id, HEX);
  Serial.print(F("Capacity: ")); Serial.println(finger.capacity);
  Serial.print(F("Security level: ")); Serial.println(finger.security_level);
  Serial.print(F("Device address: ")); Serial.println(finger.device_addr, HEX);
  Serial.print(F("Packet len: ")); Serial.println(finger.packet_len);
  Serial.print(F("Baud rate: ")); Serial.println(finger.baud_rate);
  finger.getTemplateCount();

  if (finger.templateCount == 0) {
    Serial.print("Sensor doesn't contain any fingerprint data. Please run the 'enroll' example.");
  }
  else {
    Serial.println("Waiting for valid finger...");
      Serial.print("Sensor contains "); Serial.print(finger.templateCount); Serial.println(" templates");
  }

}

uint8_t readnumber(void) {
  uint8_t num = 0;

  while (num == 0) {
    while (! Serial.available());
    num = Serial.parseInt();
  }
  return num;
}
void loop() {

    if(Serial.available()> 0){
      bdoi = Serial.read();
      switch (bdoi){
        case 'E':{
          Serial.println("*****************");
          Serial.println("Student Registration");
          dangkysv();
          break;
        }
        case 'P':{
          Serial.println("*******************");
          Serial.println("Student Check Attendance");
          diemdanhsv();
          break;
        }
        case 'Y':{
          finger.emptyDatabase();
          break;
        }
        
      }
     // break;
    }
 
  }
  //*********************************

  void dangkysv(){     //Doan so I Chinh***********************************
  Serial.println("Ready to enroll a fingerprint!");
  Serial.println("Please type in the ID # (from 1 to 127) you want to save this finger as...");
  id = readnumber();
  if (id == 0) {// ID #0 not allowed, try again!
     return;
  }
  Serial.print("Enrolling ID #");
  Serial.println(id);  
  //getFingerprintEnroll();
  while (!  getFingerprintEnroll() );
    }            //Ket thuc doan so I Chinh***********************************************
    uint8_t getFingerprintEnroll() {         //Doan so 2 ****************************************

  int p = -1;
  Serial.print("Waiting for valid finger to enroll as #"); Serial.println(id);
  while (p != FINGERPRINT_OK) {         //Doan 2A ***************************************************
    p = finger.getImage();
    switch (p) {
    case FINGERPRINT_OK:
      Serial.println("Image taken");
      break;
    case FINGERPRINT_NOFINGER:
      Serial.println(".");
      break;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      break;
    case FINGERPRINT_IMAGEFAIL:
      Serial.println("Imaging error");
      break;
    default:
      Serial.println("Unknown error");
      break;
    }
  }          //Ket thuc doan 2A ***************************************************************************

  // OK success!

  p = finger.image2Tz(1);
  switch (p) {        //Doan 2B**************************************************************************
    case FINGERPRINT_OK:
      Serial.println("Image converted");
      break;
    case FINGERPRINT_IMAGEMESS:
      Serial.println("Image too messy");
      return p;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      return p;
    case FINGERPRINT_FEATUREFAIL:
      Serial.println("Could not find fingerprint features");
      return p;
    case FINGERPRINT_INVALIDIMAGE:
      Serial.println("Could not find fingerprint features");
      return p;
    default:
      Serial.println("Unknown error");
      return p;
  }                     //Ket thuc doan 2B**************************************************************

  Serial.println("Remove finger");
  delay(100);
  p = 0;
  while (p != FINGERPRINT_NOFINGER) {        //Doan 2C**********************************************************
    p = finger.getImage();
  }                          //Ket thuc doan 2C *************************************************************
  Serial.print("ID "); Serial.println(id);
  p = -1;
  Serial.println("Place same finger again");
  while (p != FINGERPRINT_OK) {         //Doan 2D *********************************************************
    p = finger.getImage();
    switch (p) {
    case FINGERPRINT_OK:
      Serial.println("Image taken");
      break;
    case FINGERPRINT_NOFINGER:
      Serial.print(".");
      break;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      break;
    case FINGERPRINT_IMAGEFAIL:
      Serial.println("Imaging error");
      break;
    default:
      Serial.println("Unknown error");
      break;
    }
  }                     //Ket thuc doan 2D ********************************

  // OK success!

  p = finger.image2Tz(2);
  switch (p) {              //Doan 2E **************************
    case FINGERPRINT_OK:
      Serial.println("Image converted");
      break;
    case FINGERPRINT_IMAGEMESS:
      Serial.println("Image too messy");
      return p;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      return p;
    case FINGERPRINT_FEATUREFAIL:
      Serial.println("Could not find fingerprint features");
      return p;
    case FINGERPRINT_INVALIDIMAGE:
      Serial.println("Could not find fingerprint features");
      return p;
    default:
      Serial.println("Unknown error");
      return p;
  }                  // Ket thuc doan 2E ***************************

  // OK converted!
  Serial.print("Creating model for #");  Serial.println(id);        

  p = finger.createModel();
  if (p == FINGERPRINT_OK) {
    Serial.println("Prints matched!");
  } else if (p == FINGERPRINT_PACKETRECIEVEERR) {
    Serial.println("Communication error");
    return p;
  } else if (p == FINGERPRINT_ENROLLMISMATCH) {
    Serial.println("Fingerprints did not match");
    return p;
  } else {
    Serial.println("Unknown error");
    return p;
  }

  Serial.print("ID "); Serial.println(id);
  p = finger.storeModel(id);
  if (p == FINGERPRINT_OK) {
    delay(100);
    Serial.println("Stored!");
    
    Serial.flush();//cho cho den khi du lieu trong bo dem duoc goi di het
    
    //Serial.readString();
    delay(100);
    
  } else if (p == FINGERPRINT_PACKETRECIEVEERR) {
    Serial.println("Communication error");
    return p;
  } else if (p == FINGERPRINT_BADLOCATION) {
    Serial.println("Could not store in that location");
    return p;
  } else if (p == FINGERPRINT_FLASHERR) {
    Serial.println("Error writing to flash");
    return p;
  } else {
    Serial.println("Unknown error");
    return p;
  }

  return true;
}                  //ket thuc doan so 2 ***************************************************
//*****************************

void diemdanhsv(){             //Doan I Chinh goi ham getFingerprintID()           
  getFingerprintID();
    //Serial.println(getFingerprintID());
      
   
      //delay(50); 
  }                           //Ket thuc doan I Chinh
  uint8_t getFingerprintID() {        //Doan so 2  bat dau Ham getFingerprintID() 
  Serial.println("Get Fingerprint !!!");
  delay(3000);
  uint8_t p = finger.getImage();         //Doan 2A goi ham finger.getImage() 
  
  Serial.println("Fingerprints have been taken already !!!");
  
  switch (p) {
    case FINGERPRINT_OK:
      Serial.println("Image taken");
      //Serial.println("FINGERPRINT_NOFINGER la : ");
       //Serial.println(FINGERPRINT_NOFINGER );                                
         
      break;
    case FINGERPRINT_NOFINGER:
      Serial.println("No finger detected");
      return p;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      return p;
    case FINGERPRINT_IMAGEFAIL:
      Serial.println("Imaging error");
      return p;
    default:
      Serial.println("Unknown error");
      return p;
  }                                   //Ket thuc doan 2A   xu ly ham finger.getImage() 

  // OK success!

  p = finger.image2Tz();                  //Doan 2B   goi ham finger.image2Tz() 
  switch (p) {
    case FINGERPRINT_OK:
      Serial.println("Image converted");
      break;
    case FINGERPRINT_IMAGEMESS:
      Serial.println("Image too messy");
      return p;
    case FINGERPRINT_PACKETRECIEVEERR:
      Serial.println("Communication error");
      return p;
    case FINGERPRINT_FEATUREFAIL:
      Serial.println("Could not find fingerprint features");
      return p;
    case FINGERPRINT_INVALIDIMAGE:
      Serial.println("Could not find fingerprint features");
      return p;
    default:
      Serial.println("Unknown error");
      return p;
  }                               //Ket thuc doan 2 B xu ly goi ham finger.image2Tz()

  // OK converted!
  p = finger.fingerSearch();             //Doan 2C goi ham finger.fingerSearch()
  if (p == FINGERPRINT_OK) {
    Serial.println("Found a print match!");
  } else if (p == FINGERPRINT_PACKETRECIEVEERR) {
    Serial.println("Communication error");
    return p;
  } else if (p == FINGERPRINT_NOTFOUND) {
    Serial.println("Did not find a match");
    return p;
  } else {
    Serial.println("Unknown error");
    return p;
  }                           //Ket thuc doan 2C Ham finger.fingerSearch()

  // found a match!
  Serial.println("Found ID #"); 
  delay(20);
  //Serial.flush();
  Serial.println(finger.fingerID);
  //Serial.flush();
  
  delay(500);
  //Serial.println(" with confidence of "); 
  //Serial.println(finger.confidence);

  return finger.fingerID;
}                      //Ket thuc doan so 2 Ham getFingerprintID() 

// returns -1 if failed, otherwise returns ID #
//int getFingerprintIDez() {                        //Doan 3 Ham getFingerprintIDez() 
  //uint8_t p = finger.getImage();
  //if (p != FINGERPRINT_OK)  return -1;

  //p = finger.image2Tz();
  //if (p != FINGERPRINT_OK)  return -1;

  //p = finger.fingerFastSearch();
  //if (p != FINGERPRINT_OK)  return -1;

  // found a match!
  //Serial.println("Found ID #");
  //delay(100);
  //Serial.println(finger.fingerID); 
  
  //Serial.flush();
  //delay(100);
  //Serial.println(" with confidence of "); 
  //Serial.println(finger.confidence);
  //return finger.fingerID;
//}                                       //Ket thuc doan 3 ham getFingerprintIDez()********************************************
