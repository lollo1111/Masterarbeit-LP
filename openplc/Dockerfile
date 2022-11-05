FROM debian:stretch
RUN apt update
RUN apt install -y git sqlite3 python-pip python2.7 sudo dos2unix
WORKDIR /workdir
RUN git clone https://github.com/thiagoralves/OpenPLC_v3.git
WORKDIR /workdir/OpenPLC_v3
RUN ./install.sh linux
COPY active_program /workdir/OpenPLC_v3/webserver
RUN rm /workdir/OpenPLC_v3/webserver/st_files/blank_program.st
COPY mbconfig.cfg /workdir/OpenPLC_v3/webserver
COPY setup.sh /workdir/OpenPLC_v3
RUN dos2unix /workdir/OpenPLC_v3/setup.sh
#CMD ["/workdir/OpenPLC_v3/webserver/scripts/start_openplc.sh"]
EXPOSE 502
EXPOSE 8080
# EXPOSE 20000
# EXPOSE 43628
CMD ["/workdir/OpenPLC_v3/setup.sh"]
# CMD ["/workdir/OpenPLC_v3/setup.sh"]