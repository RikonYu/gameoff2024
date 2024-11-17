
import os
from PIL import Image

def extract_gif_frames():
    gif_files = [f for f in os.listdir('.') if f.endswith('.gif')]
    if not gif_files:
        print("No .gif files found in the current directory.")
        return
    for gif_file in gif_files:
        fname = os.path.splitext(gif_file)[0]
        output_dir = os.path.join('.', fname)
        if not os.path.exists(output_dir):
            os.makedirs(output_dir)
        print(f"Processing {gif_file}...")
        try:
            gif = Image.open(gif_file)
            frame_count = 0
            while True:
                if frame_count >= 100:
                    print(f"Warning: {gif_file} has more than 100 frames. Only the first 100 frames will be processed.")
                    break
                frame_filename = os.path.join(output_dir, f"{fname}_{frame_count + 1:03d}.png")
                gif.save(frame_filename, format="PNG")
                frame_count += 1
                try:
                    gif.seek(gif.tell() + 1)
                except EOFError:
                    break
            print(f"Processed {gif_file}, {frame_count} frames extracted.")
        except Exception as e:
            print(f"Error processing {gif_file}: {e}")

if __name__ == "__main__":
    extract_gif_frames()