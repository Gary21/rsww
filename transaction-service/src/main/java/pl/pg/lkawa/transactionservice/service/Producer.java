package pl.pg.lkawa.transactionservice.service;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.beans.factory.config.ConfigurableBeanFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.stereotype.Service;
import pl.pg.lkawa.transactionservice.model.Transaction;

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

    public void send(boolean transactionResult, String queueName) throws JsonProcessingException {
        LOGGER.info("Thread ID %d : Queue Name: %s\t Message sent -> %s".formatted(Thread.currentThread().getId(), queueName, transactionResult));

        byte[] bytes = new byte[] {(byte) (transactionResult ? 1 : 0)};

        rabbitTemplate.convertAndSend(queueName, bytes);
    }

}
