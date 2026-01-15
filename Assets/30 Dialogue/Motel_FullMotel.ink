INCLUDE globals.ink

Hallo! Ein Zimmer für heute Nacht bitt-#speaker:marcus #emotion:NORMAL
Sind ausgebucht. #speaker:owner
Äh, Verzeihung? #speaker:marcus
Sind ausgebucht. Was gibt's da nicht zu verstehen? #speaker:owner
Es stehen doch kaum Autos draußen! Wie könnt ihr da ausgebucht sein? #speaker:marcus #emotion:NORMAL
In welcher Welt lebst du denn? Schon mal was von Multirooming gehört, Kumpel? #speaker:owner
Multi-Was? #speaker:marcus #emotion:NERVOUS
Das ist, wenn man sich mal richtig was gönnen will. Dann bucht man sich halt nicht ein, sondern gleich drei Zimmer. #speaker:owner
... #speaker:marcus #emotion:NORMAL

#pause:Larry kommt in den Screen

Mit deinen dreckigen Klamotten wirst du dir das eh nicht leisten können. Also was soll's. #speaker:owner
Ich verbitte mir diesen Ton! Was ist das hier für ein Motel??#speaker:marcus #emotion:ANGRY
Ich kann reden wie ich will, da hast du dahergelaufener Penner nichts zu melden! Und jetzt zisch endlich ab! #speaker:owner
Wer will denn auch in so einem Ranzladen übernachten! Ich jedenfalls nicht!#speaker:marcus #emotion:ANGRY
Auf Wiedersehen!

#pause:Marcus schlägt die Tür zu und kommt raus

Whoa! Was ging denn bei euch grade ab? #speaker:larry #emotion:NERVOUS
Der Idiot hinter dem Tresen denkt, dass ihm die Welt gehört. Das ist los! #speaker:marcus #emotion:ANGRY
Hey, moment mal! Bist du nicht der Typ, dem ich vorhin den Kaffee übergeschüttet habe? #speaker:larry #emotion:HAPPY
... #speaker:marcus #emotion:NERVOUS
Ja, der bin ich. #emotion:NORMAL
Du scheinst ja grade ein wenig in der Klemme zu stecken, so ganz ohne Unterkunft. #speaker:larry #emotion:NORMAL
Weißt du was? Ich will die Sache mit den Flecken wieder gut machen. Du kannst also bei mir zuhause übernachten! #emotion:HAPPY
 * Ja
    -> Ja
 * Nein
    ->Nein
 ===Ja===
 ~ larryHomeStayDecline = false
 Immerhin ist einer hier vernünftig. Ich nehme Ihr Angebot dankend an. #speaker:marcus #emotion:NORMAL
 Super! Dann geh ich schon mal vor! Ich wohne nur ein Stück die Straße weiter unten. #speaker:larry #emotion:HAPPY
 
 #pause: Larry geht alleine die Straße runter
 
 Da hab ich ja noch mal Glück gehabt... #speaker:marcus #emotion:NORMAL
 
 ->END
 
 ===Nein===
 ~ larryHomeStayDecline = true
Danke, aber ich weiß mir schon selber zu helfen. #speaker:marcus #emotion:NORMAL
Okay, falls du es dir doch anders überlegst, kannst du gerne nachkommen. Ich wohne die Straße weiter unten. #speaker:larry #emotion:NORMAl

#pause: Larry geht alleine die Straße runter

... #speaker:marcus #emotion:SAD
    -> END
