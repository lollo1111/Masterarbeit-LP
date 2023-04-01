#!/bin/bash
# DIR="/workdir/server/"
# if [ -d "$DIR" ]; then
#   echo "Directories already exist ..."
# else
#   cpee new server
#   cpee-instantiation start
# fi
cpee new server
# cpee-instantiation start
# mv properties.init server/resources/; 
cd server ; ./cpee start ; cd .. ; cpee ui
# cd start ; ./instantiation start ; cpee ui