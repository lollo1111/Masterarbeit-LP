PROGRAM testprogramm
  VAR
    startsensor_1 AT %IX100.0 : BOOL;
    endsensor_1 AT %IX100.1 : BOOL;
    antrieb_1 AT %QX100.0 : BOOL;
    startsensor_2 AT %IX100.2 : BOOL;
    endsensor_2 AT %IX100.3 : BOOL;
    antrieb_2 AT %QX100.1 : BOOL;
    startsensor_3 AT %IX100.4 : BOOL;
    antrieb_3 AT %QX100.2 : BOOL;
  END_VAR
  VAR
    SR0 : SR;
    SR1 : SR;
    SR2 : SR;
  END_VAR
  VAR
    newantrieb AT %QX100.3 : BOOL;
    newsensor AT %IX100.6 : BOOL;
    palettedrinnen AT %IX100.7 : BOOL;
    gedreht AT %IX101.0 : BOOL;
    dreher AT %QX100.4 : BOOL;
    rfid AT %QX100.5 : BOOL;
    rfid2 AT %QX100.6 : BOOL;
  END_VAR
  VAR
    SR3 : SR;
  END_VAR
  VAR
    trigger_rfid AT %IX100.5 : BOOL;
    secondtrigger AT %IX101.1 : BOOL;
  END_VAR
  VAR
    TON0 : TON;
    delay : TIME := T#1s;
    currenttime : TIME := T#0s;
  END_VAR
  VAR
    readdata AT %IW100.0 : INT;
  END_VAR
  VAR
    controlvalue : INT := 1;
  END_VAR
  VAR
    direction AT %QW100.0 : INT;
  END_VAR
  VAR
    EQ20_OUT : BOOL;
    AND33_OUT : BOOL;
    OR21_OUT : BOOL;
    EQ36_OUT : BOOL;
    AND34_OUT : BOOL;
  END_VAR

  EQ20_OUT := EQ(readdata, controlvalue);
  AND33_OUT := AND(newsensor, EQ20_OUT);
  SR3(S1 := palettedrinnen, R := AND33_OUT);
  OR21_OUT := OR(SR3.Q1, gedreht);
  newantrieb := OR21_OUT;
  SR0(S1 := startsensor_1, R := endsensor_1);
  antrieb_1 := SR0.Q1;
  SR1(S1 := startsensor_2, R := endsensor_2);
  antrieb_2 := SR1.Q1;
  TON0(IN := palettedrinnen, PT := delay);
  SR2(S1 := startsensor_3, R := TON0.Q);
  antrieb_3 := SR2.Q1;
  EQ36_OUT := EQ(direction, controlvalue);
  AND34_OUT := AND(newsensor, EQ36_OUT);
  dreher := AND34_OUT;
  currenttime := TON0.ET;
  rfid := trigger_rfid;
END_PROGRAM


CONFIGURATION Config0

  RESOURCE Res0 ON PLC
    TASK task0(INTERVAL := T#20ms,PRIORITY := 0);
    PROGRAM instance0 WITH task0 : testprogramm;
  END_RESOURCE
END_CONFIGURATION
