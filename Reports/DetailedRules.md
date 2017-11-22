## Time system
The game runs in round. A central clock defines the game ticks. At each tick, all the instances are updated.

## Credit system

At each tick, each player receives an amount BASE_CREDIT_QTY of credits. Depending on his actions, he is awarded multipliers.

Multipliers are pairs of integers. They are always associated with time in the sense that they are temporary instances. The amount N received by
the player after each tick is multiplied by the sum of all his acquired multipliers.


Example|1|2|3|4
----------------|-------|-------|-------|-----
Multipliers     |	+20%	| +30%	| +60%	  | +10%
Validity times  | t1+30 |	t2+30 |	t3+50	|t3+60


For the next tick, the player receives N * (100(base multiplier) + 20 + 30 + 60 + 10)

t_x is the validity time of the multiplier. If the current tick is bigger than t_x, then the multiplier is discarded from the list.

## Scoring system

At each tick, teams are awarded a score based on the number of zones they control.

Score points are awarded to the players for honourable behaviour. They increase the total level of the player. Score increase is done by awarding a multiple of the SCORE_BASE_REWARD. The multiples are fixed and are defined as follows:

Multiplier | Value
-----------|------
BASE_SCORE_MULTIPLIER | 100%
KILO_SCORE_MULTIPLIER | 200%
MEGA_SCORE_MULTIPLIER | 300%
GIGA_SCORE_MULTIPLIER | 400%
TERA_SCORE_MULTIPLIER | 500%

Usually, all behaviour awards credits as an encouragement to play more. Unlocking medals and achievements awards score points and sometimes also credits.

### Capture mechanics
Attack terminals have three properties:

1. Strength (0-1000): More strength means more damage dealt to the attacked base.
2. Health
3. Level: Equal to the level of the player that placed the attack terminal. The higher the level, the higher the initial strength of the attack terminal.

Players increase the strength of allied terminal by buffing them.
Players decrease the health of ennemy terminals by de-buffing them.

If its health reaches 0, the terminal is destroyed.

Zones have two properties:

1. Health
2. Level

Players increase the health (but not the max health limit) of an allied zone by buffing it during an attack.
Players increase the level and max health limit of an allied zone by buffing it whenever it is not under attack.

A zone is captured by the team that dealt the most damage when its health reaches 0.
At each tick, attack terminals deal damage to the enemy zone for an amount proportional to their strength.
At each tick, zones deal damage to the attack terminals for an amount proportional to their level.

#### Note:
Automatic damages dealt by zones should not be too high as to encourage players to prioritize de-buffing attack terminals.

## Levels
Levels are the principal indicator of progression in the game. They are reached by accumulating score points and they confer a slight advantage. Note that the advantage should not be so high that it decourage newcomers.

### Players

SCORE |LEVEL | PERK
------------|----|---------
10k | I | Initial attack terminal strength is increased, heal quantity is increased, credits per unit of time is increased
25k | II | Initial attack terminal strength is increased, heal quantity is increased, credits per unit of time is
...|...|...

### Zones

BUFF SCORE | LEVEL | PERK
------------|------|-------
0 | NULL | -
50k | I | Auto defense against ennemy terminals, deals BASE_AUTODEF_DMG
100k | II | BASE_AUTODEF_DMG is increased, BASE_MAX_HEALTH is increased
200k | III | BASE_AUTODEF_DMG is increased, BASE_MAX_HEALTH is increased
500k | IV | BASE_AUTODEF_DMG is increased, BASE_MAX_HEALTH is increased. Auto heal after attacks, heals BASE_AUTOHEAL_QTY
1m | V | BASE_AUTODEF_DMG is increased, BASE_MAX_HEALTH is increased, BASE_AUTOHEAL_QTY is increased


## Multipliers, medals and achievements

### Multipliers

 DESCRIPTION | MULTIPLIER
------------|-------------
Planting an attack terminal | MUTLIPLIER_ATTACK_BASE (200%, 60s)
Someone buffed your terminal | MULTIPLIER_ATTACK_STACK (10%, 30s)
Buff an attack terminal | MULTIPLIER_BUFF_TERMINAL (10%, 30s)
Buff a base | MULTIPLER_BUFF_BASE (10%, 30s)

#### Defense mode
* Each terminal buffed by the player confers a multiplier. (Analogy with clickers and entities that generate more resources the more you click on it) MULTIPLIER_BUFF_TERMINAL
* Each base buffed by the player confers a multiplier. MULTIPLIER_BUFF_BASE

### Medals
Medals rewards out-of-the-ordinary behaviour, they can be received an unlimited number of times.

 MEDAL NAME | DESCRIPTION | MULTIPLIER | SCORE MULTIPLIER
------------|-------------|------------|------
Fearless | Initiate an attack | - | KILO_SCORE_MULTIPLIER
Crowdfunded | Terminal was buffed by X fellow teammates during an attack | MEDAL_CROWDFUNDED_MULTIPLIER (+200%) | MEGA_SCORE_MULTIPLIER
Workforce | Buffed a terminal which then lead to a successful capture | MEDAL_WORKFORCE_MULTIPLIER (+100%, 300s) | KILO_SCORE_MULTIPLIER
Divide and conquer | Buffed attack terminals in 2 different zones | MEDAL_DIVANDCONQ_MULTIPLIER (+30%, 60s) | KILO_SCORE_MULTIPLIER
|||
Therapist | Healed  X health points of a base | MEDAL_THERAPIST_MULTIPLIER (+100%, 120s) | MEGA_SCORE_MULTIPLIER
Patron | De-buffed a terminal which ended being destroyed | MEDAL_DEFENDER_MULTIPLIER (+100%, 120s) | MEGA_SCORE_MULTIPLIER
Guardian | Repell an attack | MEDAL_GUARDIAN_MULTIPLIER (+100%, 120s) | GIGA_SCORE_MULTIPLIER
Not on my watch | Repell an attack which had 3 or more terminals | MEDAL_NOTONMYWATCH_MULTIPLIER (+400%, 300s) | TERA_SCORE_MULTIPLIER
|||
Gambler | Spend X resources in T seconds | - | BASE_SCORE_MULTIPLIER
Big-time gambler | Spend 3X resources in 3T seconds | - | KILO_SCORE_MULTIPLIER
Addicted gambler | Spend 5X resource in 5T seconds | - | MEGA_SCORE_MULTIPLIER
Benefactor | Invest X resources in levelling a base | - | BASE_SCORE_MULTIPLIER
Shareholder | Invest 3X resources in levelling a base | - | KILO_SCORE_MULTIPLIER
Capitalist | Invest 5X resources in levelling a base | MEDAL_CAPITALIST_MULTIPLIER(+100%) | MEGA_SCORE_MULTIPLIER



### Achievements
Achievements are an additional opportunity of progression in the game. They can be received only once and they award score and perks. Perks are used to modify the role of the player the team and tune it according to its preferred way of playing.

ACHIEVEMENT NAME | DESCRIPTION | SCORE MULTIPLIER | PERK
------------|-------------|------------|------
Soldier I | Used X attack terminals | GIGA_SCORE_MULTIPLIER | BASE_ATTACK_TERMINAL_STRENGTH is increased
Soldier II | Used X attack terminals | GIGA_SCORE_MULTIPLIER | BASE_ATTACK_TERMINAL_STRENGTH is increased
Soldier III | Used X attack terminals | GIGA_SCORE_MULTIPLIER | BASE_ATTACK_TERMINAL_STRENGTH is increased
Soldier IV | Used X attack terminals | GIGA_SCORE_MULTIPLIER | BASE_ATTACK_TERMINAL_STRENGTH is increased
Soldier V | Used X attack terminals | GIGA_SCORE_MULTIPLIER | BASE_ATTACK_TERMINAL_STRENGTH is increased
Defender I | De-buffed ennemy terminals for X points  |  GIGA_SCORE_MULTIPLIER | BASE_DEBUFF_QTY is increased
Defender II | De-buffed ennemy terminals for X points  |  GIGA_SCORE_MULTIPLIER | BASE_DEBUFF_QTY is increased
Defender III | De-buffed ennemy terminals for X points  |  GIGA_SCORE_MULTIPLIER | BASE_DEBUFF_QTY is increased
Defender IV | De-buffed ennemy terminals for X points  |  GIGA_SCORE_MULTIPLIER | BASE_DEBUFF_QTY is increased
Defender V | De-buffed ennemy terminals for X points  |  GIGA_SCORE_MULTIPLIER | BASE_DEBUFF_QTY is increased
Medic I | Healed X health points |  GIGA_SCORE_MULTIPLIER | BASE_HEAL is increased
Medic II | Healed X health points |  GIGA_SCORE_MULTIPLIER | BASE_HEAL is increased
Medic III | Healed X health points |  GIGA_SCORE_MULTIPLIER | BASE_HEAL is increased
Medic IV | Healed X health points |  GIGA_SCORE_MULTIPLIER | BASE_HEAL is increased
Medic V | Healed X health points |  GIGA_SCORE_MULTIPLIER | BASE_HEAL is increased
Support I | Buffed attack terminals for X points | GIGA_SCORE_MULTIPLIER | BASE_BUFF_TERMINAL is increased
Support II | Buffed attack terminals for X points | GIGA_SCORE_MULTIPLIER | BASE_BUFF_TERMINAL is increased
Support III | Buffed attack terminals for X points | GIGA_SCORE_MULTIPLIER | BASE_BUFF_TERMINAL is increased
Support IV | Buffed attack terminals for X points | GIGA_SCORE_MULTIPLIER | BASE_BUFF_TERMINAL is increased
Support V | Buffed attack terminals for X points | GIGA_SCORE_MULTIPLIER | BASE_BUFF_TERMINAL is increased

## On-the-spot bonuses
Visual cues appear randomly on the map. When the player clicks on them, he receives a small amount of points and score. They are not geolocalised.

## Raids

Raids are a secondary game objective. They are an additional incentive for the player to physically move.
Raids are geolocalised instances that provide resources and score for the players that complete them as well as their team. In order to complete a raid, players have to place attack terminals in the raid zone just like when capturing regular zones. The group of player which damaged the raid instance the most receives the rewards.

Once a raid has been completed by a team, it is unavailable for a fixed amount of time.
