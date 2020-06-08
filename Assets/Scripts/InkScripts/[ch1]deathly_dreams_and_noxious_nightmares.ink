VAR player_name = "Lydia"
VAR barkeeper = "Fred"
VAR barkeeper_wife = "Mabel"
VAR herbalist = "Emilia"
VAR priest = "Father Gregor"
VAR wizard = "Kasien"
 
I trudged through the forest, wind whipped my hair and chilled my skin.#forest#Lydia#Kasien#Emilia#Fred#Mabel#Jasper#Gregoire
My legs ached, it was a torment I was quite familiar with. As a soldier I'd marched for days with no sleep, little food or water.
I'd fought for my country and when it was all over they tossed me aside like a blunted sword.
There was peace now, no one knew for how long. At least what the kings considered peace. We still had to pick up the pieces, restore the aftermath.
For me there was no rest. Now I was just another man, I had no income, no food, no roof over my head. I was forced to move from town to town seeking refuge and work. 
People were not as friendly as I'd hoped, but that was life.
I navigated rocky terrain, uncertain of my bearings. I spotted a stream and knew this was my chance. There were usually settlings by water sources, if I could get to the end of this one I would strike gold.
As I travelled my throat grew dry. The cold crisp water of the stream was so tempting. I'd been drinking stale water from my pack for days.

*[drink water from stream] ->drank_from_stream
*[drink water from pack] -> shortcut_to_village


{player_name}? #Received

==drank_from_stream==
It's running water, of course it's safe. And what's the point in wasting the water in my pack when there's fresh water here. I crouch down, cup my hands together and drink deeply from the stream.->END
==shortcut_to_village==
Of course I don't know the quality of the water, what minerals have been deposited in it. If a goat has taken a shit in it recently. Even if it is running water, I'd rather not take the risk right now.
I arrived at a village. It's tiny, only a handful of buildings and huts. It has a quaint feel to it. Something about it sent a shiver down my spine. Quaint but unsettlingly quiet.
I better go to the inn, talk to the barkeeper, they always know what's going on, I'm sure they can direct me to someone who needs work.
->inn

// ==where_to_go==
// +[Inn]->inn
// +[Apothecary] -> apothecary
// +[Church] -> church

==inn==
The inn was small, the smallest I've seen, and lit by many candles. A couple of men huddled over thick oak tables, which resembled slabs of wood slapped onto wooden pillars rather than furniture. There was minimal decoration, everything was crudely designed, made for utility rather than decoration. 

The only woman is the barmaid who was very modestly dressed, she had a matronly air about her. I couldn't imagine her luring in men with her appearance and flirtatious manner. She must have been the barkeeper's wife.
->at_the_inn_01

==at_the_inn_01==
*Buy a drink 
I passed the barkeeper a coin and he poured me a mug of dark liquid. I took a sip. It was slighlty sweet, fruity and a bit earthy. It was probably the best ale I'd ever had.
->at_the_inn_01
* Talk to the barkeeper 
{player_name}: "Hello good man, what do they call you?"#Inn#Fred#Lydia
???: "I won't be giving my name to no stranger"
{player_name}: "Well that can easily be sorted, I'm {player_name}, I was a soldier."
I show him the cheap coppery medal they gave me as some kind of shit compensation
???: Well I suppose it won't hurt to tell you, I'm {barkeeper}
{player_name}: Nice to meet you sir
{barkeeper}: There's no work here, maybe check the farmhouse, they always need an extra pair of hands. But you'll get little coin, we're not rich folk around here.
{player_name}: That's ok, I just need a roof over my head.
{barkeeper}: I'd sort it out quickly if I were you, it gets dark fast over here.
I wonder what he means.
{barkeeper}: You might want to visit the apothecary first, buy your medicine.
->barkeep_explains
==barkeep_explains==
*{!drank_from_stream}-> ignorant_of_curse

->ignorant_of_curse
==ignorant_of_curse==
{player_name}: I think you're mistaken sir, I'm not sick
{barkeeper}: You don't know? Well you see the water here is slightly poisonous, so we take this medicine and it stops us from getting sick
I wasn't sure what to say, perhaps he was making fun of me. I'd never heard of such a thing.
{barkeeper}: Don't you worry, once you take your medicine it'll be fine.
I still couldn't tell if he was taking the piss or not. But it was getting dark and I had no time to loiter.
*[Go to farmhouse]->farmhouse_01
*[Go to apothecary]->apothecary_01
==apothecary_01==
The barkeeper's comments had piqued my interest, so I rushed straight to the apothecary.
->END
==church_01==
->END
==farmhouse_01==
Work and a roof over my head was still the most important thing on my mind so I rushed to the farmhouse.
->END
#{conext_score == 5:->progress_conext_complete}
#{conext_score <5: ->please_try_again}



