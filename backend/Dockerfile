FROM node:16-alpine

WORKDIR /usr/src/app
COPY package*.json ./
RUN npm install
RUN apk update && apk add docker
COPY . .
EXPOSE 9033
CMD [ "node", "server.js" ]