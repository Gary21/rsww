package pl.pg.lkawa.transactionservice.component;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.validation.constraints.NotNull;
import java.util.Random;

@Component
public class NumberGenerator {
    private static final Random random = new Random();

    @NotNull
    @Value("${transaction.max-response-time}")
    private int maxResponseTime;

    public int generate(){
        return random.nextInt(maxResponseTime);
    }
}
