# Character Audio System - Setup Guide

## Gambaran Umum

Sistem audio karakter ini menambahkan sound effects untuk:
- **Lompat (Jump)** - Dimainkan saat karakter melompat
- **Mendarat (Landing)** - Dimainkan saat karakter mendarat setelah melompat
- **Langkah Kaki (Footsteps)** - Dimainkan saat karakter berjalan, berlari, atau sprint

## Instalasi Otomatis

Sistem akan otomatis menambahkan `CharacterAudioController` component ke karakter Anda saat game dimulai. Anda hanya perlu menambahkan audio clips.

## Setup Manual (Opsional)

Jika ingin setup secara manual:

1. Pilih GameObject karakter Anda (yang memiliki `vThirdPersonInput` component)
2. Klik **Add Component** di Inspector
3. Cari dan tambahkan **Character Audio Controller**

## Konfigurasi Audio Clips

### 1. Jump & Landing Sounds

Di Inspector, buka section **Jump & Landing Sounds**:

- **Jump Sound**: AudioClip yang dimainkan saat karakter melompat
  - Volume: 0.7 (default)
  
- **Landing Sound**: AudioClip yang dimainkan saat karakter mendarat
  - Volume: 0.6 (default)

### 2. Footstep Sounds

Buka section **Footstep Sounds**:

#### Walk (Berjalan)
- **Walk Footsteps**: Array berisi beberapa AudioClip langkah kaki untuk variasi
  - Akan dipilih secara random untuk suara yang lebih natural
  - Volume: 0.5 (default)
  - Step Interval: 0.5 detik (default)

#### Run (Berlari)
- **Run Footsteps**: Array berisi AudioClip untuk berlari
  - Volume: 0.6 (default)
  - Step Interval: 0.35 detik (default)

#### Sprint
- **Sprint Footsteps**: Array berisi AudioClip untuk sprint
  - Jika kosong, akan menggunakan Run Footsteps
  - Volume: 0.7 (default)
  - Step Interval: 0.25 detik (default)

### 3. Advanced Settings

- **Min Input For Footsteps**: Minimum input magnitude untuk memainkan footsteps (default: 0.1)
- **Use Random Pitch**: Menambahkan variasi pitch untuk suara lebih natural (default: true)
- **Pitch Variation**: Jumlah variasi pitch (default: 0.1)

## Cara Mendapatkan Audio Clips

### Opsi 1: Asset Store (Gratis)
- **FootstepsSoundPack**: Berbagai suara langkah kaki
- **Casual Game SFX**: Termasuk jump dan landing sounds
- **Free Sound Effects Pack**: Koleksi sound effects gratis

### Opsi 2: Situs Gratis
- [Freesound.org](https://freesound.org)
  - Cari: "footstep grass", "jump", "landing"
  - Filter: Creative Commons 0 (CC0) untuk penggunaan bebas
  
- [Mixkit.co](https://mixkit.co/free-sound-effects/)
  - Kategori: Game Sounds
  
- [Zapsplat.com](https://www.zapsplat.com)
  - Gratis dengan atribusi

### Opsi 3: Buat Sendiri
Gunakan software audio seperti:
- **Audacity** (gratis)
- **FL Studio** (berbayar)
- **Reaper** (trial gratis)

## Contoh Konfigurasi

### Setup Minimal
```
Jump Sound: jump_sound.wav
Landing Sound: land_sound.wav
Walk Footsteps: [footstep1.wav, footstep2.wav]
Run Footsteps: [run_step1.wav, run_step2.wav]
```

### Setup Lengkap dengan Variasi
```
Jump Sound: jump_grass.wav
Landing Sound: land_grass.wav

Walk Footsteps: 
  - footstep_grass_01.wav
  - footstep_grass_02.wav
  - footstep_grass_03.wav
  - footstep_grass_04.wav

Run Footsteps:
  - run_grass_01.wav
  - run_grass_02.wav
  - run_grass_03.wav
  - run_grass_04.wav

Sprint Footsteps:
  - sprint_grass_01.wav
  - sprint_grass_02.wav
  - sprint_grass_03.wav
```

## Testing

### Test di Play Mode
1. Masuk ke Play Mode
2. Di Inspector Character Audio Controller, akan muncul tombol:
   - **Test Jump Sound**
   - **Test Landing Sound**
3. Klik untuk mendengar preview sound

### Test In-Game
1. Jalankan game
2. Bergerak dengan WASD - dengarkan footsteps
3. Tekan Shift - dengarkan perubahan kecepatan footsteps (sprint)
4. Tekan Space - dengarkan jump sound
5. Mendarat - dengarkan landing sound

## Tips & Best Practices

### Volume Balance
- Jump/Landing: 0.6 - 0.7 (medium-loud)
- Walk: 0.4 - 0.5 (soft)
- Run: 0.5 - 0.6 (medium)
- Sprint: 0.6 - 0.7 (medium-loud)

### Variasi Suara
- Minimal 2-3 footstep variations per tipe movement
- Lebih banyak variasi = suara lebih natural
- Gunakan random pitch untuk variasi tambahan

### Step Intervals
- Walk: 0.4 - 0.6 detik
- Run: 0.3 - 0.4 detik
- Sprint: 0.2 - 0.3 detik

### Audio File Format
- Format: WAV atau OGG
- Sample Rate: 44100 Hz
- Bit Depth: 16-bit
- Mono untuk footsteps (lebih ringan)
- Stereo untuk ambient/music

## Troubleshooting

### Tidak ada suara sama sekali
1. Cek apakah AudioListener ada di scene (biasanya di Main Camera)
2. Cek volume di Audio Mixer
3. Cek apakah AudioClips sudah di-assign

### Footsteps tidak main
1. Cek **Min Input For Footsteps** tidak terlalu tinggi
2. Pastikan karakter grounded (isGrounded = true)
3. Cek console untuk error messages

### Suara terlalu pelan/keras
1. Adjust volume per-sound di Inspector
2. Atau adjust AudioSource.volume di script
3. Cek Audio Mixer settings

### Footsteps terlalu cepat/lambat
1. Adjust Step Interval values
2. Walk Step Interval: untuk berjalan
3. Run Step Interval: untuk berlari
4. Sprint Step Interval: untuk sprint

## Customization

### Mengubah Logic via Code

```csharp
// Mendapatkan reference
CharacterAudioController audioController = GetComponent<CharacterAudioController>();

// Mengubah volume
audioController.SetMasterVolume(0.8f);

// Stop semua audio
audioController.StopAllAudio();

// Manual play sounds
audioController.PlayJumpSound();
audioController.PlayLandingSound();
```

### Menambahkan Surface-Based Footsteps

Untuk footsteps berbeda per surface (grass, wood, metal, etc), Anda bisa extend script:

1. Duplicate `CharacterAudioController.cs`
2. Tambahkan system untuk detect surface type
3. Gunakan array footsteps berbeda per surface

## Integration dengan Audio Mixer

1. Buat Audio Mixer Group untuk "Character SFX"
2. Assign AudioSource.outputAudioMixerGroup di Inspector
3. Control volume via mixer untuk kontrol global

## Performance Notes

- Footsteps menggunakan `PlayOneShot()` - efficient untuk multiple sounds
- Random pitch variation minimal impact pada performance
- Untuk multiple characters, setiap AudioSource independent

## Credits

System ini diintegrasikan dengan Invector Third Person Controller.
Dibuat untuk menambahkan audio feedback pada karakter.

## Support

Jika ada pertanyaan atau issue:
1. Cek console untuk error messages
2. Pastikan AudioClips format-nya compatible (WAV/OGG)
3. Verify Inspector settings match documentation
