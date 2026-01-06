INCLUDE globals.ink

{ state0 == 0: -> Markus_Frau | -> Markus_Sohn }

=== Markus_Frau ===
Marcus Handy beginnt zu klingeln. #speaker:narrator
Es war seine Ex-Frau.
Für einen Moment überlegt Marcus, ob er rangehen soll.
+ [Rangehen]
    ~ state0 = 1
    -> call
+ [Auflegen]
    -> Ende

=== Ende ===    
Marcus drückt den Anruf weg und betritt den Laden.
-> END

=== Markus_Sohn ===
Marcus Handy beginnt wieder zu klingeln. #speaker:narrator
Es war erneut seine Ex-Frau.
Für einen Moment überlegt Marcus, ob er rangehen soll.

+ [Rangehen]
    -> call_sohn
+ [Auflegen]
    -> Ende


=== call ===
Hallo? #speaker:marcus #emotion:NORMAL
Hallo? Marcus? #speaker:marcus_handy #emotion:NORMAL
Was gibt's?  #speaker:marcus #emotion:NORMAL
Wann kommst du mal wieder nach Hause? #speaker:marcus_handy #emotion:NORMAL
Leon vermisst dich schon.
Ich hab doch gesagt am Ende der Woche! #speaker:marcus #emotion:ANGRY
Frustriert legt Marcus auf. #speaker:narrator
-> DONE

=== call_sohn ===
Hallo? #speaker:marcus #emotion:NORMAL
Hallo, Dad? #speaker:marcus_handy #emotion:NORMAL
Was gibt's, Leon?  #speaker:marcus #emotion:NORMAL
Wann kommst du mal wieder nach Hause? #speaker:marcus_handy #emotion:NORMAL
... #speaker:marcus #emotion:ANGRY
Frustriert legt Marcus auf. #speaker:narrator
-> DONE