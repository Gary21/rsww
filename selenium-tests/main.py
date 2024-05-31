from properties import *
from selenium import webdriver
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.service import Service as ChromeService
from selenium.webdriver.support import expected_conditions as EC
import time
import string

class Tester:

    driver = None
    
    def __init__(self, driver):
        self.driver = driver

    def set_fields_on_main(self):
        # OPEN DESTINATION LIST
        destination_i_tag = driver.find_element(By.XPATH, '/html/body/div[1]/div/div/main/div[3]/div[1]/div[1]/div[2]/div[1]/div/div[4]/i')
        destination_i_tag.click()
        
        # CHOOSE NOWY JORK
        nowy_jork_div = driver.find_element(By.XPATH, '/html/body/div[2]/div/div/div/div[2]')
        nowy_jork_div.click()
        
        time.sleep(0.5)
        
        # OPEN DEPARTURE LIST
        departure_i_tag = driver.find_element(By.XPATH, '/html/body/div[1]/div/div/main/div[3]/div[1]/div[2]/div[2]/div[1]/div/div[4]/i')
        departure_i_tag.click()
        
        # CHOOSE GDANSK
        gdansk_div = driver.find_element(By.XPATH, '/html/body/div[2]/div/div/div/div[2]')
        gdansk_div.click()
        
        time.sleep(0.5)
        
        # OPEN CALENDAR
        calendar_div = driver.find_element(By.XPATH, '//*[@id="app"]/div/div/main/div[3]/div[1]/div[3]/div[2]/div[1]/div/div[3]/div')
        calendar_div.click()
        
        time.sleep(0.5)
        
        # CHOOSE 05/22/2024
        specific_date_button = driver.find_element(By.XPATH, '/html/body/div[2]/div/div/div/div[1]/div[2]/div/div[32]')
        specific_date_button.click()
        
        ok_button = driver.find_element(By.XPATH, '/html/body/div[2]/div/div/div/div[2]/button[2]')
        ok_button.click()
        
        time.sleep(0.5)
        
        # ADD ADULT
        adult_plus_input = driver.find_element(By.XPATH, '/html/body/div[1]/div/div/main/div[3]/div[2]/div[1]/div[2]/div[1]/div/div[5]/div/button/span[3]/i')
        adult_plus_input.click()
        
        time.sleep(0.5)
        
        # CLICK BOOK NOW 
        book_now_button = driver.find_element(By.XPATH, '/html/body/div[1]/div/div/main/div[3]/div[3]/div/button')
        book_now_button.click()
        
        time.sleep(2)

    def start(self):
        driver.get(get_main_url())
        self.set_fields_on_main()   


# service = ChromeService(executable_path=ChromeDriverManager().install())
# driver = webdriver.Chrome(service = ChromeDriverManager().install())
# driver = webdriver.Chrome()
service = ChromeService(ChromeDriverManager().install())
driver = webdriver.Chrome(service=service)

tester = Tester(driver)
tester.start()

driver.quit()