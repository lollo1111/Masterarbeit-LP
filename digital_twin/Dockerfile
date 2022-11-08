FROM node:16-alpine

WORKDIR /usr/src/app
ADD digital_twin/ .
RUN npm install
EXPOSE 5173

CMD [ "npm", "run", "dev" ]