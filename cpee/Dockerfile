FROM debian:bullseye
WORKDIR /workdir
ADD setup.sh .
RUN apt-get update && apt-get install -y \
    sudo \
    ruby \
    build-essential \
    ruby-dev \
    libxml2-dev \
    libxslt-dev\
    libz-dev \
    libssl-dev \
    librasqal-dev \
    libraptor2-dev \
    libicu-dev \
    redis
RUN gem install cpee cpee-instantiation
CMD ["./setup.sh"]