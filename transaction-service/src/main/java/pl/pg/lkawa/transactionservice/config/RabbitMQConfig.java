package pl.pg.lkawa.transactionservice.config;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.msgpack.jackson.dataformat.MessagePackFactory;
import org.springframework.amqp.core.*;
import org.springframework.amqp.rabbit.config.SimpleRabbitListenerContainerFactory;
import org.springframework.amqp.rabbit.connection.ConnectionFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class RabbitMQConfig {

    @Value("${rabbitmq.queue.exchange_name}")
    private String exchangeName;

    @Value("${rabbitmq.queue.incoming_transactions_queue}")
    private String incomingTransactionsQueueName;

    @Value("${rabbitmq.queue.incoming_routing_key}")
    private String incomingRoutingKey;

    @Value("${rabbitmq.concurrent-consumers}")
    private int concurrentConsumers;

    @Value("${rabbitmq.max-concurrent-consumers}")
    private int maxConcurrentConsumers;

    @Bean
    Queue incomingTransactionsQueue(){
        return new Queue(incomingTransactionsQueueName, false);
    }

    @Bean
    TopicExchange exchange(){
        return new TopicExchange(exchangeName);
    }

    @Bean
    Binding incomingTransactionsBinding(Queue incomingTransactionsQueue, TopicExchange exchange){
        return BindingBuilder
                .bind(incomingTransactionsQueue)
                .to(exchange)
                .with(incomingRoutingKey);
    }

    @Bean
    ObjectMapper transactionMapper(){
        return new ObjectMapper(new MessagePackFactory());
    }

    @Bean
    public SimpleRabbitListenerContainerFactory rabbitListenerContainerFactory(ConnectionFactory connectionFactory) {
        SimpleRabbitListenerContainerFactory factory = new SimpleRabbitListenerContainerFactory();
        factory.setConcurrentConsumers(concurrentConsumers);
        factory.setMaxConcurrentConsumers(maxConcurrentConsumers);
        factory.setConnectionFactory(connectionFactory);
        factory.setConsecutiveActiveTrigger(1);
        factory.setAcknowledgeMode(AcknowledgeMode.NONE);
        return factory;
    }

}
