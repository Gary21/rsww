package org.example;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ExampleController {

    @Value("${instance.id}")
    private String instanceId;

    @GetMapping("getInstanceId")
    public String getInstanceId(){
        return instanceId;
    }

}
