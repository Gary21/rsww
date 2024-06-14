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
      <td>{{ preferences['DestinationCountry'][key]['ReservationCount'] }}</td>
      <td>{{ preferences['DestinationCountry'][key]['PurchaseCount'] }}</td>
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
      <td>{{ preferences['HotelName'][key]['ReservationCount'] }}</td>
      <td>{{ preferences['HotelName'][key]['PurchaseCount'] }}</td>
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
      <td>{{ preferences['DestinationCity'][key]['ReservationCount'] }}</td>
      <td>{{ preferences['DestinationCity'][key]['PurchaseCount'] }}</td>
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
      <td>{{ preferences['RoomType'][key]['ReservationCount'] }}</td>
      <td>{{ preferences['RoomType'][key]['PurchaseCount'] }}</td>
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
      <td>{{ preferences['TransportType'][key]['ReservationCount'] }}</td>
      <td>{{ preferences['TransportType'][key]['PurchaseCount'] }}</td>
    </tr>
  </tbody>
</table>

</template>

<script>
export default {
data: () => ({
    preferences: {
  "DestinationCountry": {
    "Hiszpania": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    }
  },
  "HotelName": {
    "Hilton2": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    },
    "Hilton3": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    },
    "Hilton4": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    },
    "Hilton5": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    },
  },
  "DestinationCity": {
    "Gdańsk2": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    }
  },
  "RoomType": {
    "Suite": {
      "ReservationCount": 2,
      "PurchaseCount": 2
    }
  },
  "TransportType": {
    "Bus4": {
      "ReservationCount": 20,
      "PurchaseCount": 20
    }
  }
},
websocket: null,
}),
methods: {
    async init_websocket() {
        var socket_url = 'ws://localhost:49940/Offers/PreferencesWebsocket'
        this.websocket = new WebSocket(socket_url)
        this.websocket.onmessage = this.onSocketMessage;
    },
    onSocketMessage(evt){
      //we parse the json that we receive
      var received = JSON.parse(evt.data);
      var new_pref = {
        "ReservationCount": received.Preference.ReservationCount,
        "PurchaseCount": received.Preference.PurchaseCount
      };
      this.preferences[received['PreferenceType']][received['PreferenceName']] = {};
      this.preferences[received['PreferenceType']][received['PreferenceName']] = new_pref;

      
      //this.preferences = received;
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