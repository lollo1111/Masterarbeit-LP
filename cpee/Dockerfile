FROM debian:bullseye
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
    redis \
    dos2unix
WORKDIR /workdir
RUN gem install cpee cpee-instantiation
COPY setup.sh .
RUN dos2unix /workdir/setup.sh
CMD ["/workdir/setup.sh"]