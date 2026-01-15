INCLUDE globals.ink

{ 
    larryHomeStayDecline:
    - true:
        -> DeclinedOffer
    - else:
        -> AcceptedOffer
}

===DeclinedOffer===
Ich wusste, dass du noch kommst! #speaker:larry #emotion:HAPPY
Ja... Ich konnte Ihrem Angebot wohl doch nicht widerstehen. #speaker:marcus #emotion:NORMAL
Tja, bei so einer Luxusbude ist das auch schwierig! Lass uns rein gehen! #speaker:larry #emotion:HAPPY
===AcceptedOffer===
Da bist du ja! Lass uns rein gehen! #speaker:larry #emotion:HAPPY
->Continue


===Continue===
#pause: Larry und Marcus gehen die Treppe rauf und stellen sich vor die Tür
Wie heißt du eigentlich? #speaker:larry #emotion:NORMAL
Marcus. Und Sie? #speaker:marcus #emotion:NORMAL
Ich bin der Laurence, aber du kannst Larry zu mir sagen!#speaker:Larry #emotion:HAPPY
Freut mich. #speaker:marcus
Und sietzen brauchen wir uns doch auch nicht, oder? Zumindestens bin ich nicht der Mensch für sowas. #emotion:NORMAL
Ich schätze, dass das in Ordnung ist. #speaker:marcus

-> END
