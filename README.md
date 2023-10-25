# sample-based-music-looper

This system, created in and for Unity 2022, is a script that will loop a song using sample values embedded within the metadata of the audio file. Currently, it only works for Vorbis .ogg files but may be expanded to include other file types in the future such as WAV. 

This project also contains an experimental script which attempts to emulate the system used to play music in the overworld in the Legend of Zelda: Breath of the Wild. This system is still very much work in progress since my understanding of the actual system is still quite limited.

# How does it work?

This system works by embedding loop points in the metadata of the audio files using a software such as Looping Audio Converter (https://github.com/libertyernie/LoopingAudioConverter/releases). This software takes in an audio file as input, allows you to label which sample is the beginning and which is the loop point. These are stored in the file as metadata called "LOOPSTART" and "LOOPLENGTH". This system then takes those values and uses them to loop the audio file while being played within Unity.

To read the metadata, this project uses the NVorbis library (https://github.com/NVorbis/NVorbis) which made the whole project much easier. My hope is to use this system for future games to allow me to compose music with an opening without having to worry about rendering two different audio files.
