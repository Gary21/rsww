package pl.pg.lkawa.transactionservice.model;

public enum TransactionStatus {
    ACCEPTED("accepted"),
    REJECTED("rejected"),
    PROCESSING("in progress");

    private final String value;

    TransactionStatus(String value){
        this.value = value;
    }

    public String getValue(){
        return value;
    }

}
