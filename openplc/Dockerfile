FROM debian:bullseye
RUN apt-get update && apt-get install -y \
    git \
    sqlite3 \
    python3-pip \
    python2.7 \
    sudo \
    dos2unix
WORKDIR /workdir
RUN git clone https://github.com/thiagoralves/OpenPLC_v3.git
WORKDIR /workdir/OpenPLC_v3
RUN ./install.sh linux
COPY active_program /workdir/OpenPLC_v3/webserver
RUN rm /workdir/OpenPLC_v3/webserver/st_files/blank_program.st
COPY mbconfig.cfg /workdir/OpenPLC_v3/webserver
COPY setup.sh /workdir/OpenPLC_v3
RUN dos2unix /workdir/OpenPLC_v3/setup.sh
EXPOSE 8080
CMD ["/workdir/OpenPLC_v3/setup.sh"]

# https://github.com/thiagoralves/OpenPLC_v3/blob/master/Dockerfile