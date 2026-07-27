/*{
  "DESCRIPTION": "A rhythmic digital-glitch pulse: horizontal bands jitter and RGB-desync on a beat, then snap back — a fast, percussive glitch look for cut-heavy edits, fully automatic.",
  "CATEGORIES": ["Guillotine", "Distortion"],
  "INPUTS": [
    { "NAME": "inputImage", "TYPE": "image" },
    { "NAME": "intensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "rate", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.25, "MAX": 8.0 }
  ]
}*/

float hash(float n) {
  return fract(sin(n * 43758.5453) * 43758.5453);
}

void main() {
  vec2 uv = isf_FragNormCoord;

  // A glitch "beat": mostly quiet, punctuated by short bursts, timed by rate.
  float beatPhase = fract(TIME * rate);
  float burst = step(0.85, hash(floor(TIME * rate)));
  float attack = smoothstep(0.0, 0.06, beatPhase) * smoothstep(0.22, 0.06, beatPhase);
  float glitch = burst * attack * intensity;

  // Coarse horizontal bands; each gets its own pseudo-random jitter for this beat.
  float band = floor(uv.y * 24.0);
  float seed = floor(TIME * rate) + band * 17.0;
  float jitter = (hash(seed) - 0.5) * 0.06 * glitch;

  vec2 shifted = vec2(uv.x + jitter, uv.y);
  vec4 base = IMG_NORM_PIXEL(inputImage, shifted);
  vec4 rSample = IMG_NORM_PIXEL(inputImage, vec2(shifted.x - jitter * 0.6, shifted.y));
  vec4 bSample = IMG_NORM_PIXEL(inputImage, vec2(shifted.x + jitter * 0.6, shifted.y));

  gl_FragColor = vec4(rSample.r, base.g, bSample.b, base.a);
}
