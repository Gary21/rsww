package pl.pg.lkawa.transactionservice.model;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.util.UUID;

@AllArgsConstructor
@NoArgsConstructor
@Builder
@Data
public class Transaction {
    private UUID id = UUID.randomUUID();
    private boolean isAccepted = false;

    public void accept(){
        isAccepted = true;
    }

    public void reject(){
        isAccepted = false;
    }
}
