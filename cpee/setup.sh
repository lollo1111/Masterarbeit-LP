#!/bin/bash

DIR="/workdir/server/"
if [ -d "$DIR" ]; then
  echo "Directories already exist ..."
else
  cpee new server
  cpee-instantiation start
fi

cd server && ./cpee start && cd .. && cd start && ./instantiation start && cpee ui