FUNCTION setFloor : INT
  VAR_INPUT
    point : INT;
    check : BOOL;
  END_VAR

  IF check THEN
    setFloor := point;
    return;
  END_IF;
END_FUNCTION

FUNCTION setFloorWithPosition : INT
  VAR_INPUT
    check : BOOL;
    sensor : BOOL;
  END_VAR

  IF check THEN
    IF sensor THEN
      setFloorWithPosition := 450;
      return;
    ELSE
      setFloorWithPosition := 80;
      return;
    END_IF;
  END_IF;
END_FUNCTION

PROGRAM program0
  VAR
    start_rfid_sensor AT %IX100.0 : BOOL;
    direction_limit_front AT %IX100.1 : BOOL;
    direction_rfid_sensor AT %IX100.2 : BOOL;
    machining_A_trigger AT %IX100.6 : BOOL;
    machining_B_remover_sensor AT %IX100.7 : BOOL;
    start_conveyor AT %QX100.0 : BOOL;
    direction_roll_forward AT %QX100.1 : BOOL;
    direction_stopper AT %QX100.2 : BOOL;
    direction_turn AT %QX100.3 : BOOL;
    direction_conveyor_forward_1_3 AT %QX100.4 : BOOL;
    direction_conveyor_forward_2_3 AT %QX100.5 : BOOL;
    start_counter AT %QW100 : INT := 0;
    elevator_1_set_point AT %QW101 : INT;
    elevator_2_set_point AT %QW102 : INT;
  END_VAR
  VAR
    maximum : INT := 10000;
    CTUD0 : CTUD;
    dummy_direction : BOOL := True;
    elevator_1_dummy : INT := 0;
    first_floor : INT := 450;
    const_zero : INT := 0;
    const_two : INT := 2;
    SR0 : SR;
  END_VAR
  VAR
    direction_limit_90 AT %IX100.3 : BOOL;
    direction_end_sensor_left AT %IX100.4 : BOOL;
    direction_end_sensor_forward AT %IX100.5 : BOOL;
  END_VAR
  VAR
    SR1 : SR;
    SR2 : SR;
  END_VAR
  VAR
    spawn_pallett_trigger_sensor AT %IX101.0 : BOOL;
    spawn_pallett_end_sensor AT %IX101.1 : BOOL;
    machining_B_trigger AT %IX101.2 : BOOL;
    spawn_pallett_material_sensor AT %IX101.3 : BOOL;
    elevator_1_right_limit AT %IX101.4 : BOOL;
    elevator_1_left_limit AT %IX101.5 : BOOL;
    first_floor_trigger_sensor AT %IX101.6 : BOOL;
    first_floor_end_sensor AT %IX101.7 : BOOL;
    elevator_2_right_limit AT %IX102.0 : BOOL;
    elevator_2_left_limit AT %IX102.1 : BOOL;
    elevator_2_end_sensor AT %IX102.2 : BOOL;
    cross_sensor AT %IX102.3 : BOOL;
    spawn_pallett_conveyor AT %QX100.6 : BOOL;
    spawn_pallett_emit AT %QX100.7 : BOOL;
    machining_B_remover AT %QX101.0 : BOOL;
    direction_conveyor_left_1_3 AT %QX101.1 : BOOL;
    direction_conveyor_left_2_3 AT %QX101.2 : BOOL;
    direction_conveyor_left_3_3 AT %QX101.3 : BOOL;
    machining_A_remover AT %QX101.4 : BOOL;
    direction_conveyor_forward_3_3 AT %QX101.5 : BOOL;
    spawn_pallett_stopper AT %QX101.6 : BOOL;
    spawn_curve_1_2 AT %QX101.7 : BOOL;
    spawn_curve_2_2 AT %QX102.0 : BOOL;
    elevator_1_forward AT %QX102.1 : BOOL;
    first_floor_conveyor_1_8 AT %QX102.2 : BOOL;
    first_floor_conveyor_2_8 AT %QX102.3 : BOOL;
    first_floor_conveyor_3_8 AT %QX102.4 : BOOL;
    first_floor_conveyor_4_8 AT %QX102.5 : BOOL;
    first_floor_conveyor_5_8 AT %QX102.6 : BOOL;
    first_floor_conveyor_6_8 AT %QX102.7 : BOOL;
    first_floor_conveyor_7_8 AT %QX103.0 : BOOL;
    first_floor_conveyor_8_8 AT %QX103.1 : BOOL;
    first_floor_stopper AT %QX103.2 : BOOL;
    elevator_2_forward AT %QX103.3 : BOOL;
    before_cross_left_conveyor AT %QX103.4 : BOOL;
    machining_B_progress AT %IW100 : INT;
    elevator_1_position AT %IW101 : INT;
    elevator_2_position AT %IW102 : INT;
  END_VAR
  VAR
    const_done : INT := 50;
    CTUD1 : CTUD;
    left_counter : INT := 0;
    spawn_counter : INT := 0;
    first_floor_counter : INT := 0;
    CTUD2 : CTUD;
    SR3 : SR;
    SR4 : SR;
    SR5 : SR;
    CTUD3 : CTUD;
    SR6 : SR;
    SR7 : SR;
    SR8 : SR;
    GT28_OUT : BOOL;
    GE32_OUT : BOOL;
    OR7_OUT : BOOL;
    NOT45_OUT : BOOL;
    AND37_OUT : BOOL;
    OR9_OUT : BOOL;
    AND10_OUT : BOOL;
    NOT49_OUT : BOOL;
    AND51_OUT : BOOL;
    AND18_OUT : BOOL;
    EQ86_OUT : BOOL;
    R_TRIG1 : R_TRIG;
    OR4_OUT : BOOL;
    R_TRIG2 : R_TRIG;
    GT61_OUT : BOOL;
    EQ90_OUT : BOOL;
    R_TRIG3 : R_TRIG;
    NOT114_OUT : BOOL;
    R_TRIG4 : R_TRIG;
    AND81_OUT : BOOL;
    R_TRIG5 : R_TRIG;
    R_TRIG6 : R_TRIG;
    EQ104_OUT : BOOL;
    NOT113_OUT : BOOL;
    AND109_OUT : BOOL;
    setFloor110_OUT : INT;
    ADD115_OUT : INT;
    EQ118_OUT : BOOL;
    OR132_OUT : BOOL;
    NOT103_OUT : BOOL;
    EQ129_OUT : BOOL;
    AND131_OUT : BOOL;
    NE121_OUT : BOOL;
    AND134_OUT : BOOL;
    OR135_OUT : BOOL;
    R_TRIG7 : R_TRIG;
    ADD204_OUT : INT;
    EQ206_OUT : BOOL;
    NOT209_OUT : BOOL;
    AND208_OUT : BOOL;
    R_TRIG8 : R_TRIG;
    GT181_OUT : BOOL;
    NOT187_OUT : BOOL;
    ADD189_OUT : INT;
    LT196_OUT : BOOL;
    AND258_OUT : BOOL;
    OR259_OUT : BOOL;
    AND262_OUT : BOOL;
    NOT216_OUT : BOOL;
    NOT253_OUT : BOOL;
    ADD254_OUT : INT;
    EQ256_OUT : BOOL;
    AND251_OUT : BOOL;
    OR260_OUT : BOOL;
    setFloorWithPosition248_OUT : INT;
    EQ243_OUT : BOOL;
    NOT125_OUT : BOOL;
    AND245_OUT : BOOL;
    ADD235_OUT : INT;
    EQ238_OUT : BOOL;
    AND240_OUT : BOOL;
    OR242_OUT : BOOL;
    NOT162_OUT : BOOL;
    ADD82_OUT : INT;
    EQ59_OUT : BOOL;
    AND241_OUT : BOOL;
    EQ128_OUT : BOOL;
    AND161_OUT : BOOL;
    OR246_OUT : BOOL;
    ADD33_OUT : INT;
    NE29_OUT : BOOL;
  END_VAR

  GT28_OUT := GT(start_counter, 0);
  start_conveyor := GT28_OUT;
  GE32_OUT := GE(start_counter, 2);
  direction_stopper := GE32_OUT;
  OR7_OUT := OR(direction_limit_90, direction_rfid_sensor);
  NOT45_OUT := NOT(direction_limit_90);
  AND37_OUT := AND(direction_limit_front, dummy_direction, NOT45_OUT);
  OR9_OUT := OR(AND37_OUT, machining_A_trigger, direction_end_sensor_left);
  SR0(S1 := OR7_OUT, R := OR9_OUT);
  direction_roll_forward := SR0.Q1;
  AND10_OUT := AND(direction_limit_front, dummy_direction);
  direction_turn := AND10_OUT;
  direction_conveyor_forward_2_3 := direction_conveyor_forward_1_3;
  NOT49_OUT := NOT(dummy_direction);
  AND51_OUT := AND(direction_limit_front, NOT49_OUT);
  SR1(S1 := AND51_OUT, R := machining_A_trigger);
  direction_conveyor_forward_1_3 := SR1.Q1;
  direction_conveyor_forward_3_3 := direction_conveyor_forward_2_3;
  direction_conveyor_left_2_3 := direction_conveyor_left_1_3;
  AND18_OUT := AND(direction_limit_front, dummy_direction);
  EQ86_OUT := EQ(left_counter, 0);
  SR2(S1 := AND18_OUT, R := EQ86_OUT);
  direction_conveyor_left_1_3 := SR2.Q1;
  direction_conveyor_left_3_3 := direction_conveyor_left_2_3;
  machining_B_remover := machining_B_remover_sensor;
  machining_A_remover := direction_end_sensor_forward;
  R_TRIG1(CLK := start_rfid_sensor);
  OR4_OUT := OR(direction_end_sensor_forward, direction_end_sensor_left);
  R_TRIG2(CLK := OR4_OUT);
  CTUD0(CU := R_TRIG1.Q, CD := R_TRIG2.Q, PV := 10000);
  start_counter := CTUD0.CV;
  GT61_OUT := GT(machining_B_progress, const_done);
  spawn_pallett_emit := GT61_OUT;
  EQ90_OUT := EQ(spawn_counter, 0);
  SR3(S1 := spawn_pallett_emit, R := EQ90_OUT);
  spawn_pallett_conveyor := SR3.Q1;
  R_TRIG3(CLK := spawn_pallett_trigger_sensor);
  NOT114_OUT := NOT(elevator_1_right_limit);
  R_TRIG4(CLK := NOT114_OUT);
  CTUD2(CU := R_TRIG3.Q, CD := R_TRIG4.Q, PV := 10000);
  spawn_counter := CTUD2.CV;
  spawn_curve_1_2 := spawn_pallett_conveyor;
  spawn_curve_2_2 := spawn_curve_1_2;
  SR4(S1 := spawn_pallett_trigger_sensor, R := spawn_pallett_material_sensor);
  spawn_pallett_stopper := SR4.Q1;
  AND81_OUT := AND(dummy_direction, direction_rfid_sensor);
  R_TRIG5(CLK := AND81_OUT);
  R_TRIG6(CLK := machining_B_trigger);
  CTUD1(CU := R_TRIG5.Q, CD := R_TRIG6.Q, PV := 10000);
  left_counter := CTUD1.CV;
  EQ104_OUT := EQ(elevator_1_dummy, 0);
  NOT113_OUT := NOT(elevator_1_right_limit);
  AND109_OUT := AND(EQ104_OUT, NOT113_OUT);
  setFloor110_OUT := setFloor(first_floor, AND109_OUT);
  elevator_1_set_point := setFloor110_OUT;
  ADD115_OUT := ADD(elevator_1_position, 1);
  EQ118_OUT := EQ(ADD115_OUT, elevator_1_set_point);
  OR132_OUT := OR(EQ118_OUT, spawn_pallett_end_sensor);
  NOT103_OUT := NOT(elevator_1_right_limit);
  EQ129_OUT := EQ(elevator_1_position, 0);
  AND131_OUT := AND(NOT103_OUT, EQ129_OUT);
  NE121_OUT := NE(elevator_1_position, 0);
  AND134_OUT := AND(elevator_1_right_limit, NE121_OUT);
  OR135_OUT := OR(AND131_OUT, AND134_OUT);
  SR5(S1 := OR132_OUT, R := OR135_OUT);
  elevator_1_forward := SR5.Q1;
  R_TRIG7(CLK := first_floor_trigger_sensor);
  ADD204_OUT := ADD(elevator_2_position, 1);
  EQ206_OUT := EQ(ADD204_OUT, 450);
  NOT209_OUT := NOT(elevator_2_right_limit);
  AND208_OUT := AND(EQ206_OUT, NOT209_OUT);
  R_TRIG8(CLK := AND208_OUT);
  CTUD3(CU := R_TRIG7.Q, CD := R_TRIG8.Q, PV := 10000);
  first_floor_counter := CTUD3.CV;
  first_floor_conveyor_2_8 := first_floor_conveyor_1_8;
  first_floor_conveyor_3_8 := first_floor_conveyor_2_8;
  first_floor_conveyor_4_8 := first_floor_conveyor_3_8;
  first_floor_conveyor_5_8 := first_floor_conveyor_4_8;
  first_floor_conveyor_6_8 := first_floor_conveyor_5_8;
  first_floor_conveyor_7_8 := first_floor_conveyor_6_8;
  first_floor_conveyor_8_8 := first_floor_conveyor_7_8;
  GT181_OUT := GT(first_floor_counter, 0);
  first_floor_conveyor_1_8 := GT181_OUT;
  NOT187_OUT := NOT(elevator_2_right_limit);
  ADD189_OUT := ADD(elevator_2_position, 1);
  LT196_OUT := LT(ADD189_OUT, 450);
  AND258_OUT := AND(NOT187_OUT, LT196_OUT);
  OR259_OUT := OR(first_floor_end_sensor, AND258_OUT);
  AND262_OUT := AND(elevator_2_end_sensor, elevator_2_right_limit);
  NOT216_OUT := NOT(elevator_2_right_limit);
  NOT253_OUT := NOT(elevator_2_forward);
  ADD254_OUT := ADD(elevator_2_position, 1);
  EQ256_OUT := EQ(ADD254_OUT, 450);
  AND251_OUT := AND(NOT216_OUT, NOT253_OUT, EQ256_OUT);
  OR260_OUT := OR(AND262_OUT, AND251_OUT);
  SR6(S1 := OR259_OUT, R := OR260_OUT);
  setFloorWithPosition248_OUT := setFloorWithPosition(SR6.Q1, elevator_2_right_limit);
  elevator_2_set_point := setFloorWithPosition248_OUT;
  EQ243_OUT := EQ(elevator_2_position, 80);
  NOT125_OUT := NOT(elevator_2_right_limit);
  AND245_OUT := AND(EQ243_OUT, NOT125_OUT);
  ADD235_OUT := ADD(elevator_2_position, 1);
  EQ238_OUT := EQ(450, ADD235_OUT);
  AND240_OUT := AND(first_floor_end_sensor, EQ238_OUT);
  OR242_OUT := OR(AND245_OUT, AND240_OUT);
  NOT162_OUT := NOT(elevator_2_right_limit);
  ADD82_OUT := ADD(elevator_2_position, 1);
  EQ59_OUT := EQ(450, ADD82_OUT);
  AND241_OUT := AND(NOT162_OUT, EQ59_OUT);
  EQ128_OUT := EQ(elevator_2_position, 80);
  AND161_OUT := AND(EQ128_OUT, elevator_2_right_limit);
  OR246_OUT := OR(AND241_OUT, AND161_OUT);
  SR7(S1 := OR242_OUT, R := OR246_OUT);
  elevator_2_forward := SR7.Q1;
  ADD33_OUT := ADD(1, elevator_2_position);
  NE29_OUT := NE(ADD33_OUT, 450);
  first_floor_stopper := NE29_OUT;
  SR8(S1 := elevator_2_end_sensor, R := cross_sensor);
  before_cross_left_conveyor := SR8.Q1;
END_PROGRAM


CONFIGURATION Config0

  RESOURCE Res0 ON PLC
    TASK task0(INTERVAL := T#20ms,PRIORITY := 0);
    PROGRAM instance0 WITH task0 : program0;
  END_RESOURCE
END_CONFIGURATION
