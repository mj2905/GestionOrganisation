# Emergency revision for SHS project

## Previous problems:
1. No clear game concept nor mechanics
* No clear duration.
* No decision regarding association with other events
* No sense of scale regarding the number of players which can participate at any given time.

## Proposed solution:
1. Abandon idea of organising event or take part in an event. It saves time and resources.
* The project is a game which is usable 24/7 either using geolocalisatoin services or not. The main concepts are explained in the following section.

# Game mechanics
The game mechanics should fulfill the below requirements. They should be finely tuned so that the overall experience is fun and addictive.

* Give a sense of progress to the player
* Provide a competitive aspect
* Provide instant gratification
* Make the player come back several times a day
* Scale well from 4 to 400 players

We introduce the following additional requirements to further constrain the mentioned aspects:

* The game should be playable casually (ie: with one finger)
* The game should not be too restrictive to the user. Particularly regarding the geolocalised aspect.

# Game description

## 1 Objective
The user joins one of the four teams, one for each EPFL faculty. By playing the user increases its personal score and the team's score. The team with the highest score wins.

## 2 Player and team performance assessment
The scores are used to display a ranking between the faculties.

## 3 Gameplay
In the game, the players can switch between two modes; **defense** and **attack**.

Defense mode does not have any particular requirement. In this mode, player can allocate their resources and help their team defend captured zones.

Attack mode require the players to be in a captured zone and have their localisation active. In this mode, the player can infiltrate ennemy territories and place markers to capture them. The player has a health bar which can be decreased by staying too long in ennemy territory or by walking on traps.

### 3.1 Scoring
Player, and more generally teams, must capture territories in order to acquire points. The territories are superposed to a map of EPFL and generally correspond to building or landmarks. The more captured zones, the more points are generated for the owning team. The points are generated on a timely basis such as to have very dynamic leaderboards.

### 3.2 Capturing
Territories can be grouped in one of the three following categories:
1. Neutral
2. Captured
3. Safe

Neutral territories can be captured by teams. In such case they become "Captured" and earn points on behalf of the teams.
Safe zones can never be captured, they represent zones from which players can launch attacks on other territories.

To capture a territory, the player must enable "Attack" mode. Upon activation, the game records their current localisation. The tracking persists until the player goes back into defense mode. To capture a zone, the player must be physically present in an ennemy territory and place an attack terminal. The attack terminal is a geolocalised marker that attacks the zone on a timely basis until it runs out of energy or is destroyed.

### 3.3 Defending
When one of the territories under the team's control is attacked, the attack terminal will be displayed on the map for all team members. To defend, the player must click on it and spend resources to destroy it.

### 3.4 Resources management
Depending on their actions, players will receive resources. Resources keep accumulating even when the player is disconnected. However, they are capped to a fixed threshold. This is an incentive for the player to connect multiple times a day. Resources can be spent in one of the following ways:

* Buffing attack terminal to increase their efficiency
* Strike ennemy attack terminal to destroy them
* Reinforcing captured territories to increase their resistance to attacks

### 3.5 Traps

* Circular trap area: Any attacker that enter such a zone lose health.
* Rectangular trap area: Any attacker that enter such a zone loses health.

### 3.6 Bonuses

* Resource bag: The player can gather additional resource when walking over resources bag in attack mode.

# Advantages of the proposed solution.
* Due to resources management, players are encouraged to reconnect multiple times a day.
* The game does not require the player to activate geolocalisation nor going outside if he does not want to.
* The game provides instant feedback and gratification. Upon connection, the player receives the resources points accumulated during offline time. For each action undertaken, the player sees how the action increases the rate at which its resources are gathered.
* The game is very simple to play, yet offers the possibility to develop complex strategies.
