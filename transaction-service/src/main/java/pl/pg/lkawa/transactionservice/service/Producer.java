package pl.pg.lkawa.transactionservice.service;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.msgpack.core.MessageBufferPacker;
import org.msgpack.core.MessagePack;
import org.msgpack.jackson.dataformat.MessagePackKeySerializer;
import org.msgpack.jackson.dataformat.MessagePackSerializerFactory;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.core.MessageProperties;
import org.springframework.amqp.rabbit.connection.CorrelationData;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.beans.factory.config.ConfigurableBeanFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Service;
import pl.pg.lkawa.transactionservice.model.Transaction;

import java.io.IOException;

@Service
public class Producer {

    private static final Logger LOGGER = LoggerFactory.getLogger(Consumer.class);

    private final RabbitTemplate rabbitTemplate;

    private final ObjectMapper transactionMapper;

    public Producer(RabbitTemplate rabbitTemplate,
                    @Qualifier("transactionMapper") ObjectMapper transactionMapper) {
        this.rabbitTemplate = rabbitTemplate;
        this.transactionMapper = transactionMapper;
    }

    public void send(boolean transactionResult, String queueName, String correlationId) throws IOException {
        LOGGER.info("Thread ID %d : Queue Name: %s\t Message sent -> %s".formatted(Thread.currentThread().getId(), queueName, transactionResult));

        MessageBufferPacker packer = MessagePack.newDefaultBufferPacker();
        packer.packBoolean(transactionResult);
        packer.close();

        MessageProperties properties = new MessageProperties();
        properties.setReplyTo(queueName);
        properties.setCorrelationId(correlationId);

        Message message = new Message(packer.toByteArray(), properties);

        rabbitTemplate.convertAndSend(queueName, message);
    }

}
