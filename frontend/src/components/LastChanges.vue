<template>
       <table class="bordered-table">

<thead>
   <tr>
      <th>
         ID
      </th>
      <th>
         Resource type
      </th>
      <th>
         Name
      </th>
      <th>
         Availability
      </th>
      <th>
         Price change
      </th>
   </tr>
</thead>
<tbody>
   <tr
      v-for="change in changes"
      :key="change.Id"
   >
     <td>{{ change.Id }}</td>
     <td>{{ change.ResourceType }}</td>
     <td>{{ change.Name }}</td>
     <td>{{ change.Availability }}</td>
     <td>{{ change.PriceChange }}</td>
   </tr>
</tbody>
</table>
</template>

<script>
export default {
data: () => ({
    changes: [],
    connection_ready: false,
    connection_error: false,
    websocket: null,
}),
methods: {
    async init_websocket() {
        var socket_url = 'ws://localhost:8080/Offers/ChangesWebsocket'
        this.websocket = new WebSocket(socket_url)
        this.websocket.onopen    = this.onSocketOpen;
        this.websocket.onmessage = this.onSocketMessage;
        this.websocket.onerror   = this.onSockerError;
    },
    onSocketMessage(evt){
      //we parse the json that we receive
      var received = JSON.parse(evt.data);
      console.log(received);
      if(this.changes.length > 9)
      {
      this.changes.splice(0, 1);
      }
      this.changes.push(received);
    },
    onSocketOpen(evt){
      this.connection_ready = true;
    },
    onSockerError(evt){
      this.connection_error = true;
    },
},
mounted() {
    this.init_websocket();
},
unmounted() {
    this.websocket.close();
}
};
</script>

<style>
.user-table {
display: flex;
justify-content: center;
align-items: center;
margin: 0 1rem;
}

table.bordered-table {
box-shadow: 4px 8px 12px rgba(28, 6, 49, 0.4);
border-collapse: collapse;
border-radius: 2rem;
border: 1px solid black;
overflow: hidden;
width: 100%;
text-align: left;
table-layout: fixed;
}

table.bordered-table th {
font-size: 20px;
}

table.bordered-table th,
table.bordered-table td {
padding: 0.5rem 2rem;
border: 1px solid black;
margin: 3px;
}

table.bordered-table th {
cursor: pointer;
}
th i {
transition: transform 0.3s ease-in-out;
}

.rotate-up {
transform: rotate(180deg); 
}

.rotate-down {
transform: rotate(0deg);
}

.table-user-name p {
font-weight: bold;
cursor: pointer;
color: rgb(86, 81, 81);
border-bottom: 2px solid rgba(255, 255, 255, 0);
display: inline;
transition: all 0.2s ease-in-out;
}

.table-user-name p:hover {
border-bottom: 2px solid black;
}
</style>