import pandas as pd
import numpy as np
import os
from scipy.spatial import distance

for filename in os.listdir("C:\Projects\SWDISK\scripts\SWDISK_TSP"):

    file_to_read = filename
    file_content = ""

    print("Wczytuję ", file_to_read)

    try:
        handler = open(file_to_read, "r")
        file_content = handler.read()
        handler.close()
    except IOError:
        print('Nie udało się wczytać pliku')
        exit()

    file_content = file_content[file_content.find("NODE_COORD_SECTION") + len("NODE_COORD_SECTION"):file_content.find("EOF")]
    file_content = file_content.strip()

    lines = [' '.join(line.split()).split(sep=" ") for line in file_content.split(sep="\n")]

    lines = np.array(lines)

    lines = lines.astype(np.float64)

    df_file = pd.DataFrame({
        "X_COORD": lines[:, 1],
        "Y_COORD": lines[:, 2]
    })

    rng = np.random.default_rng(12345)

    x_ratios = rng.integers(low=-1 * lines[:, 1].max(), high=lines[:, 1].max(), size=len(lines[:, 1]))
    y_ratios = rng.integers(low=-1 * lines[:, 2].max(), high=lines[:, 2].max(), size=len(lines[:, 2]))

    x_coords_transformed = [round(x_ratio + lines[:, 1][i], 2) for i, x_ratio in enumerate(x_ratios)]
    y_coords_transformed = [round(y_ratio + lines[:, 2][i], 2) for i, y_ratio in enumerate(y_ratios)]

    df_transformed = pd.DataFrame({
        "X_COORD": x_coords_transformed,
        "Y_COORD": y_coords_transformed
    })

    throughput_matrix = np.zeros(shape=(len(x_coords_transformed), len(x_coords_transformed)), dtype=np.float64)

    createFile = True

    for row_index, row in enumerate(throughput_matrix):
        for column_index, column in enumerate(row):
            if row_index != column_index:
                file_point_distance = distance.euclidean((df_file.iloc[row_index]["X_COORD"], df_file.iloc[row_index]["Y_COORD"]), (df_file.iloc[column_index]["X_COORD"], df_file.iloc[column_index]["Y_COORD"]))
                transformed_point_distance = distance.euclidean((df_transformed.iloc[row_index]["X_COORD"], df_transformed.iloc[row_index]["Y_COORD"]),(df_transformed.iloc[column_index]["X_COORD"], df_transformed.iloc[column_index]["Y_COORD"]))
                if(file_point_distance != 0):
                    throughput_matrix[row_index, column_index] = transformed_point_distance / file_point_distance
                else:
                    createFile = False
                    print("Dzielenie przez zero, transformed_point_distance = ", transformed_point_distance, "file_point_distance = ", file_point_distance)


    if(createFile):
        file_to_write = open(file_to_read+"_transformed.txt", "w+")
        file_to_write.seek(0)
        file_to_write.truncate()

        for index, row in df_transformed.iterrows():
            file_to_write.write(str(row['X_COORD'])+' '+str(row['Y_COORD'])+'\n')

        file_to_write.write('\n\n\n')

        for row_index, row in enumerate(throughput_matrix):
            str_row = [str(number) for number in row]
            file_to_write.write("\t".join(str_row)+'\n')

        file_to_write.close()
        print("Utworzono plik ", file_to_write)
