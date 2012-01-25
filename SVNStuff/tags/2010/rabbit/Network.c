//NETWORKING INFORMATION
// x = team number
// Robot is 10.0.x.2
// Team router is 10.0.x.1
#define USER_PRIMARY_NETMASK		"255.255.255.0"

// Competition router is 10.0.0.1
#define COMP_WIFI_SSID				"PIECompetition"
#define COMP_PRIMARY_NETMASK		"255.255.0.0"
#define COMP_GATEWAY					"10.0.0.1"


// Static IP configuration macros
#define TCPCONFIG 1
//#define WIFI_USE_WPA

// Set defaults to avoid warning
#define _PRIMARY_STATIC_IP				"0.0.0.0"
#define IFC_WIFI_SSID					COMP_WIFI_SSID
#define IFC_WIFI_ROAM_ENABLE  		0
#define IFC_WIFI_ROAM_BEACON_MISS	20
#define IFC_WIFI_MODE         		IFPARAM_WIFI_INFRASTRUCTURE
#define IFC_WIFI_REGION       		IFPARAM_WIFI_REGION_AMERICAS


//Ports
#define INTERFACE_PORT	3141
#define FIELD_PORT		3140

//Specific IPs
#define REMOTE_IP			"0"     //first to connect
#define FIELD_COMP_IP			"10.0.0.254"

#define MAX_UDP_SOCKET_BUFFERS 2


#use "dcrtcp.lib"


udp_Socket interface_sock;
udp_Socket field_sock;

//char interBuf[128];  MOVED TO main.c
//char fieldBuf[10];



int ConnectToNetwork(int network, unsigned char teamNo) {

	auto char ip[16], netmask[16], gateway[16], ssid[33], teamStr[4];
   int retval;

   if (sock_init()) {
   	printf("sock_init() failed\n");
      exit(-NETERR_IFDOWN);
   }

   // Bring the interface down to set the IP and gateway
	ifdown(IF_WIFI0);
   while (ifpending(IF_WIFI0) == IF_COMING_DOWN) {
		tcp_tick(NULL);
	}

	if ((retval = ifpending(IF_WIFI0)) != IF_DOWN) {
      // This should never happen, at least not with the current
      // driver.  But it doesn't hurt to check
      printf("Network did not go down, alert Rabbit! retval=%d\n", retval);
      exit(0);
   }

   // Convert team number to string
   itoa(teamNo, teamStr);

   // Set the robot IP to "10.0.#.2"
   strcpy(ip, "10.0.");
   strcat(ip, teamStr);
   strcpy(gateway, ip);
   strcat(ip, ".2");
   // Set the gateway to "10.0.#.1" (only used for USER_ROUTER)
   strcat(gateway, ".1");

	if (network == USER_ROUTER) {
      // Set the wireless SSID to "PIETeam#"
      strcpy(ssid, "PIETeam");
      strcat(ssid, teamStr);

		ifconfig(IF_WIFI0,
      			IFS_IPADDR, aton(ip),
               IFS_NETMASK, aton(USER_PRIMARY_NETMASK),
               IFS_ROUTER_SET, aton(gateway),
               IFS_NAMESERVER_SET, aton(gateway),
               IFS_WIFI_SSID, 0, ssid,
               IFS_UP,
               IFS_END);
   } else if (network == COMP_ROUTER) {
   	ifconfig(IF_WIFI0,
					IFS_IPADDR, aton(ip),
               IFS_NETMASK, aton(COMP_PRIMARY_NETMASK),
               IFS_ROUTER_SET, aton(COMP_GATEWAY),
               IFS_NAMESERVER_SET, aton(COMP_GATEWAY),
               IFS_WIFI_SSID, 0, COMP_WIFI_SSID,
               IFS_UP,
               IFS_END);
   } else {
   	printf("Not a valid router to connect to");
      exit(-NETERR_IFDOWN);
	}

   // Wait for interface to come back up
	while (ifpending(IF_DEFAULT) == IF_COMING_UP) {
		tcp_tick(NULL);
   }

   // Print ip and netmask for debugging
   inet_ntoa(ip, _if_tab[IF_DEFAULT].ipaddr);
   inet_ntoa(netmask, _if_tab[IF_DEFAULT].mask);
   printf("Network default interface up at IP=%s  mask=%s\n", ip, netmask);

   if(!udp_open(&interface_sock, INTERFACE_PORT, resolve(REMOTE_IP), 0, NULL)) {
		printf("interface port failed!\n");
		exit(0);
	}

   //If competition, only allow the competition IP
   if (network == COMP_ROUTER) {
	   if(!udp_open(&field_sock, FIELD_PORT, resolve(FIELD_COMP_IP), 0, NULL)) {
	      printf("field port failed!\n");
	      exit(0);
	   }
   } else {
      if(!udp_open(&field_sock, FIELD_PORT, 0, 0, NULL)) {
			printf("field port failed!\n");
         exit(0);
		}
   }

   // Initialize receive buffers
   memset(fieldBuf, 0, sizeof(fieldBuf));
   memset(interBuf, 0, sizeof(interBuf));

   return 1;
}


int udpRecieveFieldPacket() {
	tcp_tick(NULL);
 	if (-1 == udp_recv(&field_sock, fieldBuf, sizeof(fieldBuf))) {
   	return 0;
	}
   return 1;
}



int udpRecieveInterfacePacket() {
	tcp_tick(NULL);
 	if (-1 == udp_recv(&interface_sock, interBuf, sizeof(interBuf))) {
   	return 0;
	}
   return 1;
}


int ProcessFieldPacket() {
//Assumes fieldBuf contains valid data

   if (fieldBuf[0] == 'F') {
      return(fieldBuf[1]);
   }

   return -1;
}

void sendInterfaceSocket(char *data, int len) {
	udp_send(&interface_sock, data, len);
}

void sendFieldSocket(char *data, int len) {
   udp_send(&field_sock, data, len);
}