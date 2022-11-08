FROM debian
ADD setup.sh .
RUN apt-get update
RUN apt-get install -y sudo
RUN sudo apt-get install -y ruby build-essential ruby-dev libxml2-dev libxslt-dev libz-dev libssl-dev librasqal-dev libraptor2-dev libicu-dev redis
RUN gem install cpee
CMD ["./setup.sh"]