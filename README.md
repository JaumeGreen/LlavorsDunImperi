# LlavorsDunImperi
Joc per torns de conquesta

Disseny inicial

MVP

hi ha estrelles a certes distancies unes d'altres
es colonitzen i tenen característiques: població, ...
es poden fer flotes, estrelles tenen producció
les flotes tenen 3 característiques: atac, defensa, colons
N atac = N de mal
D defensa = D/2% de defensa
limit 100 de cada
mitja lluita inici de torn, mitja lluita final de torn.

colons/població... inici població 1000
producció sense res = població /10, minim 1
fabrica cost 10, genera 2/torn, limit 100
flota atac 1/1
flota defensa 1/1
flota colons 1/1+1de població

estrelles tenen canvi poblacional, entre -1 i +10
estrelles inicials tots +7
resta estan buides

torn rebut:
posició estrella (x,y,z)
població P
fabriques F
flotes A/D/C

Flotes en transit
origen
destí

estrelles properes
x,y,z (sense activitat, activitat detectada si hi ha algú)

Torn enviat:
estrella x,y,z
construir N fabriques
augmentar flota S en A/D/C

flota T anar a xyz
