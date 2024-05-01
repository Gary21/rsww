package pl.pg.lkawa.transactionservice.component;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;
import pl.pg.lkawa.transactionservice.model.Transaction;
import pl.pg.lkawa.transactionservice.model.TransactionStatus;

import java.util.UUID;

@Component
public class Runner implements CommandLineRunner {

    @Value("${rabbitmq.queue.exchange_name}")
    private String topicExchangeName;

    @Value("${rabbitmq.queue.incoming_transactions_queue}")
    private String queueName;

    @Value("${rabbitmq.queue.incoming_routing_key}")
    private String incomingRoutingKey;

    private final RabbitTemplate rabbitTemplate;
    private final ObjectMapper transactionMapper;

    public Runner(RabbitTemplate rabbitTemplate,
                  @Qualifier("transactionMapper") ObjectMapper transactionMapper) {
        this.rabbitTemplate = rabbitTemplate;
        this.transactionMapper = transactionMapper;
    }

    @Override
    public void run(String... args) throws Exception {
        Transaction transaction = Transaction.builder()
                .id(UUID.randomUUID())
                .status(TransactionStatus.PROCESSING)
                .build();
        byte[] bytes = transactionMapper.writeValueAsBytes(transaction);
        rabbitTemplate.convertAndSend(topicExchangeName, incomingRoutingKey, bytes);
        rabbitTemplate.convertAndSend(topicExchangeName, incomingRoutingKey, bytes);
        rabbitTemplate.convertAndSend(topicExchangeName, incomingRoutingKey, bytes);
        rabbitTemplate.convertAndSend(topicExchangeName, incomingRoutingKey, bytes);
    }

}
