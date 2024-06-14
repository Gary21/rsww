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
     <td>{{ change.availability }}</td>
     <td>{{ change.priceChange }}</td>
   </tr>
</tbody>
</table>
</template>

<script>
export default {
data: () => ({
    changes: [],
websocket: null,
}),
methods: {
    async init_websocket() {
        var socket_url = 'http://localhost:49940/Offers/ChangesWebsocket'
        this.websocket = new WebSocket(socket_url)
        this.websocket.onmessage = this.onSocketMessage;
    },
    onSocketMessage(evt){
      //we parse the json that we receive
      var received = JSON.parse(evt.data);
      console.log(received);
      this.changes = received;
    },
},
mounted() {
    this.init_websocket();
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