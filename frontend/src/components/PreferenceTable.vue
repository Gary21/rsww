<template>

<!-- DESTINATION COUNTRY -->
Destination country:
<table class="bordered-table">
  <thead>
    <tr>
        <th>
          Destination
        </th>
        <th>
          Reservation count
        </th>
        <th>
          Rurchase count
        </th>
    </tr>
  </thead>
  <tbody>
    <tr
        v-for="(key) in Object.keys(preferences['DestinationCountry'])"
    >
      <td>{{ key }}</td>
      <td>{{ preferences['DestinationCountry'][key]['reservationCount'] }}</td>
      <td>{{ preferences['DestinationCountry'][key]['purchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

<!-- HOTEL NAME -->
Hotel name:
<table class="bordered-table">
  <thead>
    <tr>
        <th>
          Hotel name
        </th>
        <th>
          Reservation count
        </th>
        <th>
          Rurchase count
        </th>
    </tr>
  </thead>
  <tbody>
    <tr
        v-for="(key) in Object.keys(preferences['HotelName'])"
    >
      <td>{{ key }}</td>
      <td>{{ preferences['HotelName'][key]['reservationCount'] }}</td>
      <td>{{ preferences['HotelName'][key]['purchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

<!-- DESTINATION CITY -->
Destination city:
<table class="bordered-table">
  <thead>
    <tr>
        <th>
          Destination city
        </th>
        <th>
          Reservation count
        </th>
        <th>
          Rurchase count
        </th>
    </tr>
  </thead>
  <tbody>
    <tr
        v-for="(key) in Object.keys(preferences['DestinationCity'])"
    >
      <td>{{ key }}</td>
      <td>{{ preferences['DestinationCity'][key]['reservationCount'] }}</td>
      <td>{{ preferences['DestinationCity'][key]['purchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

<!-- ROOM TYPE -->
Room type:
<table class="bordered-table">
  <thead>
    <tr>
        <th>
          Room type
        </th>
        <th>
          Reservation count
        </th>
        <th>
          Rurchase count
        </th>
    </tr>
  </thead>
  <tbody>
    <tr
        v-for="(key) in Object.keys(preferences['RoomType'])"
    >
      <td>{{ key }}</td>
      <td>{{ preferences['RoomType'][key]['reservationCount'] }}</td>
      <td>{{ preferences['RoomType'][key]['purchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

<!-- TRANSPORT TYPE -->
Transport type:
<table class="bordered-table">
  <thead>
    <tr>
        <th>
          Transport type
        </th>
        <th>
          Reservation count
        </th>
        <th>
          Rurchase count
        </th>
    </tr>
  </thead>
  <tbody>
    <tr
        v-for="(key) in Object.keys(preferences['TransportType'])"
    >
      <td>{{ key }}</td>
      <td>{{ preferences['TransportType'][key]['reservationCount'] }}</td>
      <td>{{ preferences['TransportType'][key]['purchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

</template>

<script>
export default {
data: () => ({
    preferences: {
  "DestinationCountry": {
    "Polska": {
      "reservationCount": 2,
      "purchaseCount": 2
    }
  },
  "HotelName": {
    "Hilton2": {
      "reservationCount": 2,
      "purchaseCount": 2
    },
    "Hilton3": {
      "reservationCount": 2,
      "purchaseCount": 2
    },
    "Hilton4": {
      "reservationCount": 2,
      "purchaseCount": 2
    },
    "Hilton5": {
      "reservationCount": 2,
      "purchaseCount": 2
    },
  },
  "DestinationCity": {
    "Gdańsk2": {
      "reservationCount": 2,
      "purchaseCount": 2
    }
  },
  "RoomType": {
    "Suite": {
      "reservationCount": 2,
      "purchaseCount": 2
    }
  },
  "TransportType": {
    "Bus4": {
      "reservationCount": 20,
      "purchaseCount": 20
    }
  }
},
websocket: null,
}),
methods: {
    async init_websocket() {
        var socket_url = 'http://localhost:8080/Offers/PreferencesWebsocket'
        this.websocket = new WebSocket(socket_url)
        this.websocket.onmessage = this.onSocketMessage;
    },
    onSocketMessage(evt){
      //we parse the json that we receive
      var received = JSON.parse(evt.data);
      console.log(received);
      this.preferences = received;
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