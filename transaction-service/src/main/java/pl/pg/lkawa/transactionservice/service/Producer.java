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

    @Value("${rabbitmq.queue.exchange_name}")
    private String exchangeName;

    @Value("${rabbitmq.queue.response_routing_key}")
    private String responseRoutingKey;

    private static final Logger LOGGER = LoggerFactory.getLogger(Consumer.class);

    private final RabbitTemplate rabbitTemplate;

    private final ObjectMapper transactionMapper;

    public Producer(RabbitTemplate rabbitTemplate,
                    @Qualifier("transactionMapper") ObjectMapper transactionMapper) {
        this.rabbitTemplate = rabbitTemplate;
        this.transactionMapper = transactionMapper;
    }

    public void send(Transaction transaction) throws JsonProcessingException {
        LOGGER.info("Thread ID %d : Message sent -> %s".formatted(Thread.currentThread().getId(), transaction));

        byte[] bytes = transactionMapper.writeValueAsBytes(transaction);

        rabbitTemplate.convertAndSend(exchangeName, responseRoutingKey, bytes);
    }

}
