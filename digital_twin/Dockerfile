FROM node:16-alpine

WORKDIR /usr/src/app
ADD . .
RUN npm install
EXPOSE 5173

CMD [ "npm", "run", "dev" ]