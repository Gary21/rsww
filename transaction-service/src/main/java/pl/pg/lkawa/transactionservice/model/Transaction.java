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
    private UUID id;
    private TransactionStatus status;

    public void accept(){
        status = TransactionStatus.ACCEPTED;
    }

    public void reject(){
        status = TransactionStatus.REJECTED;
    }
}
