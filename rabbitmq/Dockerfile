FROM rabbitmq:3-management

ENV RABBITMQ_DEFAULT_USER rabbitmq
ENV RABBITMQ_DEFAULT_PASS rabbitmq
ENV RABBITMQ_PID_FILE /var/lib/rabbitmq/mnesia/rabbitmq
ENV RABBITMQ_DEFAULT_VHOST /


ADD rabbitmq.conf /etc/rabbitmq/rabbitmq.conf

ADD definitions.json /etc/rabbitmq/

RUN chown rabbitmq:rabbitmq /etc/rabbitmq/rabbitmq.conf /etc/rabbitmq/definitions.json
VOLUME /var/lib/rabbitmq

RUN rabbitmq-plugins enable --offline rabbitmq_shovel && rabbitmq-plugins enable --offline rabbitmq_shovel_management

EXPOSE 15672
EXPOSE  5672
