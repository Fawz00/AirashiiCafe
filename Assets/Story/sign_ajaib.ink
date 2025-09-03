->  encounter_npc

=== encounter_npc ===
# speaker: Narrator
# background: background_noon
The land festers in ruin. Smoke clings to crumbled stone, and distant screams echo beneath blood-red skies.
"You’re new here, aren’t ya? Picked a cursed time to wander these soils. This country bleeds dry—best vanish before you’re next."
+ [Why?] -> why_chaos
+ [Ignore them.]
-> leave

=== why_chaos ===
"The ‘Green Keeper’ rules here now. A tyrant of vine and rot. Strong as an iron bull, sly as poison. If he sees you and you ain't his kin? Your throat’s forfeit."
+ [Is there any way I can help?] -> help_wounded
+ [That’s not my problem.]
-> leave

=== help_wounded ===
# event: finding_mushroom
"Our wounded won’t last till dusk. Fetch us 5 mushrooms from Fuerta Forestal. They sprout where sap runs red. They’re the only balm left for Ashenborn and Stonebound alike."
->END

=== leave ===
"Chhh, another coward"
->END