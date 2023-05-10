#!/bin/bash

stop_cpee() {
  ./cpee stop
  pkill cpee
}

trap stop_cpee SIGTERM
cpee new server
cpee-instantiation instantiation
cd server ; ./cpee start ; cd .. ; cd instantiation ; ./instantiation start ; cd .. ; cpee ui ; cd server
wait $!