# BbSprite

[![BbSprite](video.png)](https://www.youtube.com/watch?v=RwGG-_eUjFk "BbSprite")
<i>A rundown of the features of the BBSprite plugin.</i>

![Screenshot 1](screenshot01.gif)

<h2>Description</h2>
<div style="margin-left:2em;">
<p style="margin-bottom:0;">A synthesizer for PC and Android inspired by the <a style="color:black;text-decoration:underline;" href="https://www.youtube.com/watch?v=fC3fD1pTyro" target="_blank">Pocket Piano instrument</a>. Features include:</p>

<ul>
 	<li><b>4 instrument slots</b> - Play multiple instruments simultaneously</li>
 	<li><b>5 waveform choices</b> - Different instrument sounds</li>
 	<li><b>Full envelope</b> - Set how sharp the instrument sounds and how fast it fades</li>
 	<li><b>Volume LFO with 4 waveforms</b> - Add different forms of vibrato</li>
 	<li><b>An easy to use arpeggiator</b> - Play many notes in rapid succession</li>
</ul>
</div>
<h2>Challenges</h2>
<div style="margin-left:2em;">

One challenge was that libGDX has no GUI functionality, requiring that I develop the UI from scratch.  The UI has many moving parts, requiring a hierarchy based UI and procedural graphics for flexibility.
<br/><br/>
A more showstopping issue was due to audio latency issues inherent in the Android OS. I was able to minimize this with precomputation and optimizing buffer sizes.  However, the project's goal of a fun, interactive toy was always hampered by audio latency. Android's audio issues are less prominent today than they used to be, however. Perhaps this app deserves a revisit.

</div>
