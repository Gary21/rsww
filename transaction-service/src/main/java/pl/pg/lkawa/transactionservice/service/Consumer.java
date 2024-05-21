package pl.pg.lkawa.transactionservice.service;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.amqp.core.Message;
import org.springframework.amqp.rabbit.annotation.RabbitListener;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;
import pl.pg.lkawa.transactionservice.component.BooleanGenerator;
import pl.pg.lkawa.transactionservice.component.NumberGenerator;
import pl.pg.lkawa.transactionservice.model.Transaction;

import java.io.IOException;
import java.util.concurrent.TimeUnit;

@Service
public class Consumer {

    private static final Logger LOGGER = LoggerFactory.getLogger(Consumer.class);

    private final Producer producer;
    private final ObjectMapper transactionMapper;
    private final BooleanGenerator booleanGenerator;
    private final NumberGenerator numberGenerator;

    public Consumer(Producer producer,
                    @Qualifier("transactionMapper") ObjectMapper transactionMapper,
                    BooleanGenerator booleanGenerator,
                    NumberGenerator numberGenerator) {
        this.producer = producer;
        this.transactionMapper = transactionMapper;
        this.booleanGenerator = booleanGenerator;
        this.numberGenerator = numberGenerator;
    }

    @Async
    @RabbitListener(queues = {"${rabbitmq.queue.incoming_transactions_queue}"},
                    concurrency = "4")
    public void consume(Message message) throws IOException {
        try {
            TimeUnit.MILLISECONDS.sleep(numberGenerator.generate());
            LOGGER.info("Thread ID %d : Received message".formatted(Thread.currentThread().getId()));
            boolean transactionResult = false;
            if (booleanGenerator.generateWithProbability() == true) {
                transactionResult = true;
            } else {
                transactionResult = false;
            }
            producer.send(transactionResult, message.getMessageProperties().getReplyTo());
        } catch (InterruptedException e){
            Thread.currentThread().interrupt();
        }
    }
}
