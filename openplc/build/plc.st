TYPE
  LOGLEVEL : (CRITICAL, WARNING, INFO, DEBUG) := INFO;
END_TYPE

FUNCTION_BLOCK LOGGER
  VAR_INPUT
    TRIG : BOOL;
    MSG : STRING;
    LEVEL : LOGLEVEL := INFO;
  END_VAR
  VAR
    TRIG0 : BOOL;
  END_VAR

  IF TRIG AND NOT TRIG0 THEN
  {{
   LogMessage(GetFbVar(LEVEL),(char*)GetFbVar(MSG, .body),GetFbVar(MSG, .len));
  }}
  END_IF;
  TRIG0:=TRIG;
END_FUNCTION_BLOCK



FUNCTION_BLOCK python_eval
  VAR_INPUT
    TRIG : BOOL;
    CODE : STRING;
  END_VAR
  VAR_OUTPUT
    ACK : BOOL;
    RESULT : STRING;
  END_VAR
  VAR
    STATE : DWORD;
    BUFFER : STRING;
    PREBUFFER : STRING;
    TRIGM1 : BOOL;
    TRIGGED : BOOL;
  END_VAR

  {extern void __PythonEvalFB(int, PYTHON_EVAL*);__PythonEvalFB(0, data__);}
END_FUNCTION_BLOCK

FUNCTION_BLOCK python_poll
  VAR_INPUT
    TRIG : BOOL;
    CODE : STRING;
  END_VAR
  VAR_OUTPUT
    ACK : BOOL;
    RESULT : STRING;
  END_VAR
  VAR
    STATE : DWORD;
    BUFFER : STRING;
    PREBUFFER : STRING;
    TRIGM1 : BOOL;
    TRIGGED : BOOL;
  END_VAR

  {extern void __PythonEvalFB(int, PYTHON_EVAL*);__PythonEvalFB(1,(PYTHON_EVAL*)(void*)data__);}
END_FUNCTION_BLOCK

FUNCTION_BLOCK python_gear
  VAR_INPUT
    N : UINT;
    TRIG : BOOL;
    CODE : STRING;
  END_VAR
  VAR_OUTPUT
    ACK : BOOL;
    RESULT : STRING;
  END_VAR
  VAR
    py_eval : python_eval;
    COUNTER : UINT;
    ADD10_OUT : UINT;
    EQ13_OUT : BOOL;
    SEL15_OUT : UINT;
    AND7_OUT : BOOL;
  END_VAR

  ADD10_OUT := ADD(COUNTER, 1);
  EQ13_OUT := EQ(N, ADD10_OUT);
  SEL15_OUT := SEL(EQ13_OUT, ADD10_OUT, 0);
  COUNTER := SEL15_OUT;
  AND7_OUT := AND(EQ13_OUT, TRIG);
  py_eval(TRIG := AND7_OUT, CODE := CODE);
  ACK := py_eval.ACK;
  RESULT := py_eval.RESULT;
END_FUNCTION_BLOCK


PROGRAM testprogramm
  VAR
    startsensor_1 : BOOL;
    endsensor_1 : BOOL;
    antrieb_1 : BOOL;
    startsensor_2 : BOOL;
    endsensor_2 : BOOL;
    antrieb_2 : BOOL;
    startsensor_3 : BOOL;
    antrieb_3 : BOOL;
  END_VAR
  VAR
    SR0 : SR;
    SR1 : SR;
    SR2 : SR;
  END_VAR
  VAR
    newantrieb : BOOL;
    newsensor : BOOL;
    palettedrinnen : BOOL;
    gedreht : BOOL;
    dreher : BOOL;
    rfid : BOOL;
    rfid2 : BOOL;
  END_VAR
  VAR
    SR3 : SR;
  END_VAR
  VAR
    trigger_rfid : BOOL;
    secondtrigger : BOOL;
  END_VAR
  VAR
    TON0 : TON;
    delay : TIME := T#1s;
    currenttime : TIME := T#0s;
  END_VAR
  VAR
    readdata : INT;
  END_VAR
  VAR
    controlvalue : INT := 11;
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
  EQ36_OUT := EQ(readdata, controlvalue);
  AND34_OUT := AND(newsensor, EQ36_OUT);
  dreher := AND34_OUT;
  currenttime := TON0.ET;
  rfid := trigger_rfid;
  rfid2 := secondtrigger;
END_PROGRAM


CONFIGURATION Config0

  RESOURCE Res0 ON PLC
    TASK task0(INTERVAL := T#20ms,PRIORITY := 0);
    PROGRAM instance0 WITH task0 : testprogramm;
  END_RESOURCE
END_CONFIGURATION
