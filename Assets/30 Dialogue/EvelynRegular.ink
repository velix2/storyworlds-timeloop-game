INCLUDE globals.ink

{introCompleted:
    - true:
        -> Available
    - else:
        Wat guckste so? Ich hab schlechte Laune.#speaker:evelyn
        Ach, nirgendwo.#speaker:marcus #emotion:NERVOUS
        -> END
}
        
=== Available ===
{evelynCoffeeGot:
    - true:
        Oh hallo. Brauchst du was von mir?#speaker:evelyn
        
    - else:
        Wat willste?#speaker:evelyn
}

-> Topics


=== Topics ===
+ {not evelynTruckYours}Gehört der Truck draußen dir?#speaker:marcus #emotion:NORMAL
    -> TruckYours
+ {evelynTruckYours}Wo fährst du hin?#speaker:marcus #emotion:NORMAL
    -> WhereYaGoing
+ {evelynWhereYaGoing}[Hundefutter aus Alaska?]
    -> DogFood
+ {not evelynCoffeeGot} [Kaffee?]
    -> Coffee
+ {evelynTruckYours and not evelynReadyForRide}[Kann ich bei dir mitfahren?]
    -> GoWith
+ {evelynReadyForRide} Lass uns losfahren.#speaker:marcus #emotion:NORMAL
    -> LetsGo
+ [Tschau.]
    -> Bye
    
    
    
=== TruckYours ===
 ~ evelynTruckYours = 1
 -> Topics
 
=== WhereYaGoing ===
{evelynCoffeeGot:
    - true:
        -> WhereYaGoing_C
    - else:
        -> WhereYaGoing_NC
}

    === WhereYaGoing_C ===
    Ich muss nach Los Angeles, um eine Lieferung Hundefutter liefern.#speaker:evelyn
    (Das bietet sich ja gut an. Da liegt [MY_HOME_TOWN] direkt auf dem Weg.)#speaker:marcus #emotion:NORMAL
    Kann ich sonst noch was für dich tun?#speaker:evelyn
    ~ evelynWhereYaGoing = true
     -> Topics
     
    === WhereYaGoing_NC ===
    Momentan nirgendwo. Ich mach grad Pause.#speaker:evelyn
    Schon klar, aber wo geht es danach hin?#speaker:marcus #emotion:NORMAL
    In den Süden.#speaker:evelyn
    (Mehr werd ich wohl nicht aus ihr rauskriegen.)#speaker:marcus #emotion:NORMAL
    -> Topics

 
=== DogFood ===
Du fährst also extra von Alaska nach L.A. für Hundefutter?#speaker:marcus #emotion:NORMAL
Ach, du weißt doch. Nur das beste für die Lieblinge der Stars.#speaker:evelyn
    -> Topics
 
 
=== GoWith ===
Der Zug fällt aufgrund des Schneesturms aus.[nl]Kann ich stattdessen bei dir mitfahren?#speaker:marcus #emotion:NORMAL
{evelynCoffeeGot:
    - true:
        -> GoWith_C
    - else:
        -> GoWith_NC
}
    === GoWith_C ===
    Ach du Armer.[nl]Natürlich kannst du bei mir mitfahren!#speaker:evelyn
    Ich fahr ungefähr ab 20:00 Uhr los.#speaker:evelyn
    
    ~ evelynReadyForRide = true
     -> Topics
     
     === GoWith_NC ===
     Geh mir nicht auf die Nerven. Ohne meinen Kaffee mach ich hier gar nichts.#speaker:evelyn
     Kein Grund so mürrisch zu sein.#speaker:marcus #emotion:NORMAL
     Hau ab. #speaker:evelyn
     ->END
 
 
=== Coffee ===
Du scheinst mir wirklich an deinem Kaffee zu hängen.#speaker:marcus #emotion:NORMAL
Ach, wat lässt dich das denken?#speaker:evelyn
Reine Intuition.#speaker:marcus #emotion:NORMAL
-> Topics


=== LetsGo ===
{evelynOutside:
    - true:
        -> LetsGo_O
    - else:
        -> LetsGo_NO
}

    === LetsGo_O ===
    Von mir aus kanns losgehen.#speaker:marcus #emototion:NORMAL
    Dann steig ma ein. Wir fahren los.#speaker:evelyn
    ~ evelynRideConfirmation = true
    ->END
    
    === LetsGo_NO ===
    Lass mich erst mein' Kaffee austrinken. #speaker:evelyn
    Um ungefähr 20:00 Uhr werd ich mich dann nach draußen begeben.[nl]Kannst ja bestimmt warten.#speaker:evelyn
    Klar.#speaker:marcu #emotion:NORMAL
    -> Topics
 
=== Bye ===
Ich geh dann mal wieder.#speaker:marcus #emotion:NORMAL
Mach dat. #speaker:evelyn
->END