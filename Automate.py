import os

# Get the directory where the script is located
app_dir = os.path.dirname(os.path.abspath(__file__))

# Construct the full paths to the ViewModels and Views directories
viewmodels_directory = os.path.join(app_dir, 'ViewModels')
views_directory = os.path.join(app_dir, 'Views')

# The name of the output file
output_file = os.path.join(app_dir, 'combined_output.txt')

def print_contents_to_file(directories, output_file):
    with open(output_file, 'w', encoding='utf-8') as outfile:
        for directory in directories:
            for root, _, files in os.walk(directory):
                for file in files:
                    if file.endswith('.cs') or file.endswith('.axaml'):
                        filepath = os.path.join(root, file)
                        outfile.write(f"{file}\n\n")
                        with open(filepath, 'r', encoding='utf-8') as infile:
                            outfile.write(infile.read() + "\n\n")

# Call the function with the ViewModels and Views directories
print_contents_to_file([viewmodels_directory, views_directory], output_file)
