package pl.pg.lkawa.transactionservice.component;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.validation.constraints.NotNull;
import java.util.Random;

@Component
public class BooleanGenerator {
    private static final Random random = new Random();

    @NotNull
    @Value("${transaction.acceptance.probability:1}")
    private double probability;

    public boolean generateWithProbability() {
        if (probability < 0 || probability > 1) {
            throw new IllegalArgumentException("Probability must be between 0 and 1");
        }
        return random.nextDouble() < probability;
    }}
